using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using app_backend_notificacao.domain.Interfaces.Services;
using app_backend_notificacao.domain.Models.WhatsApp;

namespace app_backend_notificacao.infrastructure.Services;

public class WhatsAppCloudApiProvider : IWhatsAppProvider
{
    private readonly HttpClient _httpClient;
    private readonly WhatsAppSettings _settings;

    public WhatsAppCloudApiProvider(HttpClient httpClient, IOptions<WhatsAppSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<bool> EnviarWhatsAppAsync(string celular, string mensagem, CancellationToken cancellationToken)
    {
        // Formatar número (remover caracteres não numéricos e garantir código do país)
        var to = FormatarNumero(celular);

        var requestBody = new WhatsAppCloudApiRequest
        {
            To = to,
            Text = new WhatsAppText { Body = mensagem }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);

        var url = $"https://graph.facebook.com/{_settings.Version}/{_settings.PhoneId}/messages";

        try
        {
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private string FormatarNumero(string numero)
    {
        // Remove tudo que não for dígito
        var limpo = new string(numero.Where(char.IsDigit).ToArray());
        
        // Se não tiver o código do país (55), adiciona (ajustar conforme necessidade)
        if (limpo.Length <= 11 && !limpo.StartsWith("55"))
        {
            limpo = "55" + limpo;
        }

        return limpo;
    }
}

