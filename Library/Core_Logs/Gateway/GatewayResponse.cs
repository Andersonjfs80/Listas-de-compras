using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Core_Logs.Gateway;

/// <summary>
/// Utilitário para padronizar respostas do Gateway no mesmo formato do Backend.
/// Possui inteligência para evitar o "double wrapping" de envelopes vindos do Backend.
/// </summary>
public static class GatewayResponse
{
    public static IResult Success(object? data, string message = "Operação realizada com sucesso", string code = "SUCCESS")
    {
        // Se o objeto já for um envelope (contém statusProcessamento), não re-envelopamos
        if (data != null && IsAlreadyEnveloped(data))
        {
            return Results.Json(data, statusCode: (int)HttpStatusCode.OK);
        }

        var response = new Dictionary<string, object?>();
        
        // Só adiciona a chave data se ela não for nula
        if (data != null)
        {
            response.Add("data", data);
        }

        response.Add("statusProcessamento", new
        {
            codigoProcessamento = code,
            mensagemProcessamento = message,
            detalhesProcessamento = (string?)null
        });

        return Results.Json(response, statusCode: (int)HttpStatusCode.OK);
    }

    public static IResult Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string code = "ERROR", string? details = null)
    {
        var response = new Dictionary<string, object?>
        {
            { "statusProcessamento", new
                {
                    codigoProcessamento = code,
                    mensagemProcessamento = message,
                    detalhesProcessamento = details
                }
            }
        };

        return Results.Json(response, statusCode: (int)statusCode);
    }

    /// <summary>
    /// Verifica se o objeto já possui a estrutura de envelope padrão.
    /// </summary>
    private static bool IsAlreadyEnveloped(object data)
    {
        if (data is JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Object && element.TryGetProperty("statusProcessamento", out _);
        }

        // Se for um objeto anônimo ou classe, verificamos via reflexão simples ou checagem de dicionário
        if (data is IDictionary<string, object> dict)
        {
            return dict.ContainsKey("statusProcessamento");
        }

        return data.GetType().GetProperty("statusProcessamento") != null || 
               data.GetType().GetProperty("StatusProcessamento") != null;
    }
}
