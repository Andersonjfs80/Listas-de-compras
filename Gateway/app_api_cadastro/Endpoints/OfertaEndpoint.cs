using Core_Http.Gateway;
using app_api_cadastro.Configuration;
using Core_Logs.Extensions;

namespace app_api_cadastro.Endpoints;

public class OfertaEndpoint : BaseGatewayEndpoint<ConfigurationSettings>
{
    public override string ServiceName => "OfertaBackend";
    public override Func<ConfigurationSettings, string> UrlSelector => settings => settings.UrlBackendOferta;

    public override void Configure(GatewayBuilder builder)
    {
        // POST /ofertas/filtrar — retorna ofertas ativas paginadas (Body payload)
        builder.Post("/ofertas/filtrar",
            new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header, Required: false)
        );
    }
}
