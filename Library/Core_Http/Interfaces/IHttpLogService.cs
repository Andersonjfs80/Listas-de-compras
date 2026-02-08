using Microsoft.AspNetCore.Http;

namespace Core_Http.Interfaces;

/// <summary>
/// Interface para registrar logs de chamadas HTTP sem depender da lib de Logs.
/// </summary>
public interface IHttpLogService
{
    void LogChamadaInterna(string url, string? request, string? response, int statusCode, long duracaoMs);
    void LogErroInterno(string url, Exception ex);
}
