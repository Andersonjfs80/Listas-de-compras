using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core_Logs.Security.Models;
using Microsoft.Extensions.Options;
using Jose;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System;
using Core_Http.Interfaces;

namespace Core_Http.Middlewares;

public class BodyEncryptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IOptions<SecuritySettings> settings, IHttpLogService logService)
    {
        var secretKey = string.IsNullOrEmpty(settings.Value.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : settings.Value.SecretKey;
        var encryptionKey = Encoding.UTF8.GetBytes(secretKey.PadRight(32).Substring(0, 32));

        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var isNonProd = env.EnvironmentName.Equals("DEV", StringComparison.OrdinalIgnoreCase) || 
                        env.EnvironmentName.Equals("HML", StringComparison.OrdinalIgnoreCase) ||
                        env.EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase) ||
                        env.EnvironmentName.Equals("Homologation", StringComparison.OrdinalIgnoreCase);

        var isEncryptionEnabled = isNonProd ? settings.Value.EnableBodyEncryption : true;

        // IGNORE SWAGGER
        if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/v1/api-docs"))
        {
            await _next(context);
            return;
        }

        // 1. Descriptografia do Request
        if (isEncryptionEnabled)
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
                                
                                logService.LogTraceCriptografia("GATEWAY_REQUEST", "DESCRIPTOGRAFAR", decryptedBody);
                                
                                var bytes = Encoding.UTF8.GetBytes(decryptedBody);
                                context.Request.Body = new MemoryStream(bytes);
                                context.Request.ContentLength = bytes.Length;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BodyEncryptionMiddleware] ERRO REQUEST: {ex.Message}");
                    if (context.Request.Body.CanSeek) context.Request.Body.Position = 0;
                }
            }
            context.Request.ContentType = "application/json";
        }

        if (!isEncryptionEnabled)
        {
            await _next(context);
            return;
        }

        // 2. Criptografia do Response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        if (context.Response.ContentType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var plainResponse = await new StreamReader(responseBody).ReadToEndAsync();
            
            if (!string.IsNullOrEmpty(plainResponse))
            {
                // LOG DE DIAGNÓSTICO
                Console.WriteLine($"[BodyEncryptionMiddleware] Response original (BFF): {plainResponse.Substring(0, Math.Min(100, plainResponse.Length))}...");

                if (plainResponse.TrimStart().StartsWith("{\"data\":", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[BodyEncryptionMiddleware] Já criptografado. Repassando...");
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                logService.LogTraceCriptografia("GATEWAY_RESPONSE", "ANTES_CRIPTOGRAFAR", plainResponse);
                
                var encryptedToken = Jose.JWT.Encode(plainResponse, encryptionKey, JweAlgorithm.DIR, JweEncryption.A256GCM);
                
                // Opções para evitar que o JSON escape caracteres do token JWE
                var options = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                var finalJsonResponse = JsonSerializer.Serialize(new { data = encryptedToken }, options);
                
                logService.LogTraceCriptografia("GATEWAY_RESPONSE", "DEPOIS_CRIPTOGRAFAR", finalJsonResponse);
                var encryptedBytes = Encoding.UTF8.GetBytes(finalJsonResponse);

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