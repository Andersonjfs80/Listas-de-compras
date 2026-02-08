using System.Text.Json.Serialization;

namespace app_backend_seguranca.domain.Models.WhatsApp;

public class WhatsAppCloudApiRequest
{
    [JsonPropertyName("messaging_product")]
    public string MessagingProduct { get; set; } = "whatsapp";

    [JsonPropertyName("recipient_type")]
    public string RecipientType { get; set; } = "individual";

    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public WhatsAppText? Text { get; set; }
}

public class WhatsAppText
{
    [JsonPropertyName("preview_url")]
    public bool PreviewUrl { get; set; } = false;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}

public class WhatsAppSettings
{
    public string Token { get; set; } = string.Empty;
    public string PhoneId { get; set; } = string.Empty;
    public string Version { get; set; } = "v20.0";
}

