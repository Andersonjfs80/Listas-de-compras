using System.Net;
using Microsoft.AspNetCore.Http;
using Core_Logs.Models;

namespace Core_Logs.Interfaces;

/// <summary>
/// Interface para serviço de logs customizados
/// </summary>
public interface ILogCustom
{
    /// <summary>
    /// Modelo consolidado do log
    /// </summary>
    LogCustomModel Log { get; }

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

    /// <summary>
    /// Consolida itens de logs externos (HttpContext.Items)
    /// </summary>
    void MapearItensExternos(HttpContext context);

    /// <summary>
    /// Envia o log consolidado para a fila do Kafka.
    /// Útil para processos que não passam pelo middleware de log (Background Tasks/Startup).
    /// </summary>
    Task EnviarLogAsync();
}
