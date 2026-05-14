using System.Text.Json.Serialization;

namespace Core_Logs.Models;

public class LogCustomModel
{
    public static string GlobalAppName { get; set; } = string.Empty;
    public static string GlobalPodName { get; set; } = string.Empty;

    [JsonPropertyName("AppName")]
    public string AppName { get; set; } = GlobalAppName;

    [JsonPropertyName("PodName")]
    public string PodName { get; set; } = GlobalPodName;

    [JsonPropertyName("Tipo")]
    public string Tipo { get; set; } = "log";

    [JsonPropertyName("Scheme")]
    public string? Scheme { get; set; }

    [JsonPropertyName("Host")]
    public string? Host { get; set; }

    [JsonPropertyName("Port")]
    public int? Port { get; set; }

    [JsonPropertyName("Method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("Url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("FullUrl")]
    public string FullUrl { get; set; } = string.Empty;

    [JsonPropertyName("Path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("RelativePath")]
    public string? RelativePath { get; set; }

    [JsonPropertyName("Query")]
    public string? Query { get; set; }

    [JsonPropertyName("Fragment")]
    public string? Fragment { get; set; }

    [JsonPropertyName("StatusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("DurationMs")]
    public long DurationMs { get; set; }

    [JsonPropertyName("TraceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("UserId")]
    public string? UserId { get; set; }

    [JsonPropertyName("RequestHeaders")]
    public Dictionary<string, string> RequestHeaders { get; set; } = new();

    [JsonPropertyName("ResponseHeaders")]
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();

    [JsonPropertyName("StackTrace")]
    public string? StackTrace { get; set; }

    [JsonPropertyName("Logs")]
    public List<string> Logs { get; set; } = new();

    [JsonPropertyName("Body")]
    public object? Body { get; set; }

    [JsonPropertyName("Response")]
    public object? Response { get; set; }
}
