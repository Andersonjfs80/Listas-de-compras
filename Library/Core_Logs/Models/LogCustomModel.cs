namespace Core_Logs.Models;

public class LogCustomModel
{
    /// <summary>
    /// Nome da aplicação global. Carregado da configuração.
    /// </summary>
    public static string GlobalAppName { get; set; } = string.Empty;

    /// <summary>
    /// Nome do Pod/Host global. Carregado da configuração.
    /// </summary>
    public static string GlobalPodName { get; set; } = string.Empty;

    /// <summary>
    /// Nome da aplicação para este log. Inicializado com o valor global.
    /// </summary>
    public string AppName { get; set; } = GlobalAppName;

    /// <summary>
    /// Nome do Pod/Host para este log. Inicializado com o valor global.
    /// </summary>
    public string PodName { get; set; } = GlobalPodName;
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? Body { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public long DurationMs { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public List<string> Logs { get; set; } = new();
}
