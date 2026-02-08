using System.Net;

namespace Core_Logs.Interfaces;

/// <summary>
/// Interface para serviço de logs customizados
/// </summary>
public interface ILogCustom
{
    /// <summary>
    /// Adiciona um log genérico
    /// </summary>
    void AdicionarLog(string mensagem, string? detalhes = null, string? stackTrace = null);
    
    /// <summary>
    /// Adiciona um log de erro
    /// </summary>
    void AdicionarErro(string mensagem, Exception ex);
    
    /// <summary>
    /// Adiciona um log de chamada HTTP
    /// </summary>
    void AdicionarLogHttp(string? url, string? request, string? response, HttpStatusCode statusCode, long duracaoMs);
}
