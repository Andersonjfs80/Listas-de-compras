namespace Core_Logs.Configuration;

/// <summary>
/// Configurações para o serviço de cache Redis
/// </summary>
public class CacheSettings
{
    public const string SectionName = "CacheSettings";

    /// <summary>
    /// String de conexão do Redis (ex: localhost:6379)
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Habilita ou desabilita o cache
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// TTL padrão em minutos para itens do cache
    /// </summary>
    public int DefaultTTLMinutes { get; set; } = 5;

    /// <summary>
    /// Habilita ou desabilita logs no console
    /// </summary>
    public bool LogConsole { get; set; } = false;
}
