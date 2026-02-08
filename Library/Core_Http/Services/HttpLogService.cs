using Core_Http.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Core_Http.Services;

/// <summary>
/// Implementação do serviço de log HTTP que usa o HttpContext.Items para manter o desacoplamento.
/// </summary>
public class HttpLogService(IHttpContextAccessor contextAccessor) : IHttpLogService
{
    private const string ItemsKey = "HttpLogMessages";

    public void LogChamadaInterna(string url, string? request, string? response, int statusCode, long duracaoMs)
    {
        Add(new
        {
            Tipo = "InternalHttp",
            Url = url,
            Request = request,
            Response = response,
            StatusCode = statusCode,
            DuracaoMs = duracaoMs,
            DataHora = DateTime.UtcNow
        });
    }

    public void LogErroInterno(string url, Exception ex)
    {
        Add(new
        {
            Tipo = "Error",
            Mensagem = $"Erro na chamada interna para {url}",
            Error = ex.Message,
            StackTrace = ex.StackTrace,
            DataHora = DateTime.UtcNow
        });
    }

    private void Add(object entry)
    {
        var context = contextAccessor.HttpContext;
        if (context == null) return;

        var list = context.Items[ItemsKey] as List<object> ?? new List<object>();
        list.Add(entry);
        context.Items[ItemsKey] = list;
    }
}
