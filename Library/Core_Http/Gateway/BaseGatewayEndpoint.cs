using Core_Logs.Extensions;
using Core_Logs.Interfaces;
using Microsoft.AspNetCore.Routing;

namespace Core_Http.Gateway;

/// <summary>
/// Classe base para definir endpoints do Gateway de forma organizada e padronizada.
/// </summary>
/// <typeparam name="TSettings">Tipo da classe de configuração do serviço (deve implementar IGatewaySettings)</typeparam>
public abstract class BaseGatewayEndpoint<TSettings>
    where TSettings : class, IGatewaySettings
{
    /// <summary>
    /// Nome do serviço de destino (usado para logs e construção de URL se necessário).
    /// </summary>
    public abstract string ServiceName { get; }

    /// <summary>
    /// Função que seleciona a URL Base do serviço a partir das configurações (ex: settings => settings.UrlBackend).
    /// </summary>
    public abstract Func<TSettings, string> UrlSelector { get; }

    /// <summary>
    /// Método onde as rotas e regras do Gateway são configuradas.
    /// </summary>
    /// <param name="builder">Builder fluente para configuração de rotas</param>
    public abstract void Configure(GatewayBuilder builder);
}

public static class GatewayEndpointExtensions
{
    /// <summary>
    /// Registra os endpoints definidos em uma classe herdeira de BaseGatewayEndpoint.
    /// </summary>
    public static void MapEndpoints<TSettings>(
        this RouteGroupBuilder group,
        BaseGatewayEndpoint<TSettings> endpoint)
        where TSettings : class, IGatewaySettings
    {
        group.MapGateway<TSettings>(
            endpoint.ServiceName,
            endpoint.UrlSelector,
            endpoint.Configure
        );
    }
}
