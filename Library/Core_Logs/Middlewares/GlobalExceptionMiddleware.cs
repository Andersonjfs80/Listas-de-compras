using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Core_Logs.Log;
using Core_Logs.Interfaces;

namespace Core_Logs.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogCustom logCustom)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Registra o detalhe do erro na lista consolidada do log
            logCustom.AdicionarErro("Exceção capturada no GlobalExceptionMiddleware", ex);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Ocorreu um erro interno no servidor.",
            Detailed = exception.Message,
            StackTrace = exception.StackTrace
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}
