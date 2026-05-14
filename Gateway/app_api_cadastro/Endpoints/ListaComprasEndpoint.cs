using Core_Http.Gateway;
using app_api_cadastro.Configuration;
using Core_Logs.Extensions;

namespace app_api_cadastro.Endpoints;

public class ListaComprasEndpoint : BaseGatewayEndpoint<ConfigurationSettings>
{
    public override string ServiceName => "ListaComprasBackend";
    public override Func<ConfigurationSettings, string> UrlSelector => settings => settings.UrlBackendListaCompras;

    public override void Configure(GatewayBuilder builder)
    {
        // GET /listas — retorna listas do usuário
        builder.Get("/listas",
            new GatewayParameter("USUARIO-ID", ParameterType.Header)
        );

        // POST /listas — cria nova lista
        builder.Post("/listas",
            new GatewayParameter("USUARIO-ID", ParameterType.Header)
        );

        // POST /listas/{listaId}/itens — adiciona item à lista
        builder.Post("/listas/{listaId}/itens",
            new GatewayParameter("listaId", ParameterType.Route),
            new GatewayParameter("USUARIO-ID", ParameterType.Header)
        );

        // PUT /listas/{listaId}/itens/{itemId}/toggle — marca/desmarca item
        builder.Put("/listas/{listaId}/itens/{itemId}/toggle",
            new GatewayParameter("listaId", ParameterType.Route),
            new GatewayParameter("itemId", ParameterType.Route),
            new GatewayParameter("USUARIO-ID", ParameterType.Header)
        );

        // DELETE /listas/{listaId}/itens/{itemId} — remove item
        builder.Delete("/listas/{listaId}/itens/{itemId}",
            new GatewayParameter("listaId", ParameterType.Route),
            new GatewayParameter("itemId", ParameterType.Route),
            new GatewayParameter("USUARIO-ID", ParameterType.Header)
        );
    }
}
