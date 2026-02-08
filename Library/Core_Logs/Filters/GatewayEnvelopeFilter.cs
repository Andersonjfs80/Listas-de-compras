using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Core_Logs.Gateway;

namespace Core_Logs.Filters;

/// <summary>
/// Filtro de Endpoint que automatiza o envelopamento de respostas no Gateway.
/// Se o endpoint retornar um objeto puro (não IResult), ele será automaticamente
/// envolvido no padrão { data: ..., statusProcessamento: ... }.
/// </summary>
public class GatewayEnvelopeFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        // Se o resultado já for um IResult (Stream, Json pronto, etc), não mexemos
        if (result is IResult || result is null)
        {
            return result;
        }

        // Se o programador retornou um objeto puro (List, Classe, Anonimo), 
        // o filtro aplica o envelope automaticamente.
        return GatewayResponse.Success(result);
    }
}

public static class GatewayEnvelopeExtensions
{
    /// <summary>
    /// Aplica o envelope automático a um grupo de endpoints ou endpoint individual.
    /// </summary>
    public static RouteGroupBuilder AddGatewayAutoEnvelope(this RouteGroupBuilder builder)
    {
        return builder.AddEndpointFilter<GatewayEnvelopeFilter>();
    }

    public static RouteHandlerBuilder AddGatewayAutoEnvelope(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<GatewayEnvelopeFilter>();
    }
}
