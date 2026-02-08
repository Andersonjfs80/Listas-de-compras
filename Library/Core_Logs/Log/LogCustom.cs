using System.Net;
using Core_Logs.Interfaces;
using Core_Logs.Models;

namespace Core_Logs.Log;

/// <summary>
/// Ancestor responsável por consolidar logs durante o ciclo de vida da requisição.
/// Deve ser registrado como Scoped.
/// </summary>
public class LogCustom : ILogCustom
{
    public LogCustomModel Log { get; } = new();

    /// <summary>
    /// Adiciona um log genérico
    /// </summary>
    public void AdicionarLog(string mensagem, string? detalhes = null, string? stackTrace = null)
    {
        var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [INFO] {mensagem}";
        if (!string.IsNullOrEmpty(detalhes)) logEntry += $" | Detalhes: {detalhes}";
        if (!string.IsNullOrEmpty(stackTrace)) logEntry += $" | StackTrace: {stackTrace}";
        
        Log.Logs.Add(logEntry);
    }

    /// <summary>
    /// Adiciona um item de log interno à lista consolidada.
    /// </summary>
    private void AdicionarLogInterno(
        string tipo, string? mensagem = null, string? request = null, 
        string? response = null, HttpStatusCode? statusCode = null, string? erro = null, 
        string? stackTrace = null, long? duracaoMs = null)
    {
        var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{tipo}] {mensagem}";
        if (statusCode.HasValue) logEntry += $" | Status: {statusCode}";
        if (duracaoMs.HasValue) logEntry += $" | Duração: {duracaoMs}ms";
        if (!string.IsNullOrEmpty(erro)) logEntry += $" | Erro: {erro}";
        
        Log.Logs.Add(logEntry);
    }

    /// <summary>
    /// Atalho para adicionar erro de chamada interna ou exceção específica.
    /// </summary>
    public void AdicionarErro(string mensagem, Exception ex)
    {
        AdicionarLogInterno("ERROR", mensagem, erro: ex.Message, stackTrace: ex.StackTrace);
    }

    /// <summary>
    /// Atalho para chamadas HTTP internas (Proxy/External API).
    /// </summary>
    public void AdicionarLogHttp(string? url, string? request, string? response, HttpStatusCode statusCode, long duracaoMs)
    {
        AdicionarLogInterno("HTTP_INTERNAL", mensagem: url, request: request, response: response, statusCode: statusCode, duracaoMs: duracaoMs);
    }

    /// <summary>
    /// Lê registros externos (como da lib Core_Http) armazenados no HttpContext.Items e os consolida no log principal.
    /// Isso permite o desacoplamento entre as libs.
    /// </summary>
    public void MapearItensExternos(Microsoft.AspNetCore.Http.HttpContext context)
    {
        const string ItemsKey = "HttpLogMessages";
        if (context?.Items[ItemsKey] is List<object> externalLogs)
        {
            foreach (var logObj in externalLogs)
            {
                 Log.Logs.Add($"[EXTERNAL] {logObj}");
            }
        }
    }
}
