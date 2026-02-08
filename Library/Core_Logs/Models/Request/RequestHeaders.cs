namespace Core_Logs.Models.Request;

/// <summary>
/// Modelo para encapsular headers padronizados da requisição
/// </summary>
public class RequestHeaders
{
    /// <summary>
    /// Sigla da aplicação cliente
    /// </summary>
    public string SiglaAplicacao { get; set; } = string.Empty;

    /// <summary>
    /// ID da sessão do usuário
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// ID único da mensagem/requisição
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Token de autorização (Bearer token)
    /// </summary>
    public string? Authorization { get; set; }

    /// <summary>
    /// ID do dispositivo do cliente
    /// </summary>
    public string? DispositivoId { get; set; }

    /// <summary>
    /// Token extraído do header Authorization (sem o prefixo "Bearer ")
    /// </summary>
    public string? Token { get; set; }
}
