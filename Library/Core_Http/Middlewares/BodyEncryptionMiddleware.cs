using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core_Logs.Security.Models;
using Microsoft.Extensions.Options;
using Jose;
using System.Text.Json;

namespace Core_Http.Middlewares;

public class BodyEncryptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IOptions<SecuritySettings> settings)
    {
        // 1. Configurações de chave
        var secretKey = string.IsNullOrEmpty(settings.Value.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : settings.Value.SecretKey;
        var encryptionKey = Encoding.ASCII.GetBytes(secretKey.PadRight(32).Substring(0, 32));

        // 2. TRATAMENTO DO REQUEST (Descriptografia Inteligente)
        var hasSecHeader = context.Request.Headers.ContainsKey("X-Sec-Key");
        
        if (settings.Value.EnableBodyEncryption && hasSecHeader)
        {
            context.Request.EnableBuffering();
            
            var bodyStream = context.Request.Body;
            var buffer = new byte[context.Request.ContentLength ?? 0];
            if (buffer.Length > 0)
            {
                await bodyStream.ReadAsync(buffer, 0, buffer.Length);
                var rawBody = Encoding.UTF8.GetString(buffer);
                bodyStream.Position = 0;

                try 
                {
                    if (!string.IsNullOrWhiteSpace(rawBody) && rawBody.TrimStart().StartsWith("{"))
                    {
                        using var jsonDoc = JsonDocument.Parse(rawBody);
                        if (jsonDoc.RootElement.TryGetProperty("data", out var dataProp))
                        {
                            var jweToken = dataProp.GetString();
                            if (!string.IsNullOrEmpty(jweToken))
                            {
                                var decryptedBody = Jose.JWT.Decode(jweToken, encryptionKey);
                                var bytes = Encoding.UTF8.GetBytes(decryptedBody);
                                
                                context.Request.Body = new MemoryStream(bytes);
                                context.Request.ContentLength = bytes.Length;
                                
                                // RECOMENDAÇÃO: Se descriptografamos, removemos o header para que
                                // downstream (Proxies/BFF) saibam que o corpo agora é um JSON comum.
                                context.Request.Headers.Remove("X-Sec-Key");
                                // Console.WriteLine("BodyEncryptionMiddleware: Corpo descriptografado e header removido.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BodyEncryptionMiddleware: ERRO NA DESCRIPTOGRAFIA: {ex.Message}");
                    if (context.Request.Body.CanSeek)
                        context.Request.Body.Position = 0;
                }
            }
            
            context.Request.ContentType = "application/json";
        }

        // 3. TRATAMENTO DO RESPONSE (Criptografia Inteligente)
        if (!settings.Value.EnableBodyEncryption)
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // Se tiver conteúdo e AINDA NÃO estiver criptografado, criptografa (independente do Status Code)
        var responseHasSecHeader = context.Response.Headers.ContainsKey("X-Sec-Key");
        
        if (!responseHasSecHeader && context.Response.ContentType?.Contains("application/json") == true)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var plainResponse = await new StreamReader(responseBody).ReadToEndAsync();
            
            if (!string.IsNullOrEmpty(plainResponse))
            {
                var encryptedToken = Jose.JWT.Encode(plainResponse, encryptionKey, JweAlgorithm.DIR, JweEncryption.A256GCM);
                
                var finalJsonResponse = JsonSerializer.Serialize(new { data = encryptedToken });
                var encryptedBytes = Encoding.UTF8.GetBytes(finalJsonResponse);

                context.Response.Headers["X-Sec-Key"] = "1";
                context.Response.ContentType = "application/json";
                context.Response.ContentLength = encryptedBytes.Length;

                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
                return;
            }
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
