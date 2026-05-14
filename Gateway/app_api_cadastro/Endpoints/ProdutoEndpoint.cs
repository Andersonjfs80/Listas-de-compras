using Core_Logs.Extensions;
using Core_Http.Gateway;
using app_api_cadastro.Configuration;
using Core_Logs.Constants;

namespace app_api_cadastro.Endpoints;

public class ProdutoEndpoint : BaseGatewayEndpoint<ConfigurationSettings>
{
    public override string ServiceName => "ProdutoBackend";
    public override Func<ConfigurationSettings, string> UrlSelector => settings => settings.UrlBackendProduto;

    public override void Configure(GatewayBuilder builder)
    {
        builder.Post("/produtos/filtrar",         
            new GatewayParameter(StandardHeaderNames.Token, ParameterType.Header)
        );

        builder.Post("/produtos",
            new GatewayParameter(StandardHeaderNames.Token, ParameterType.Header, Required: false)
        );

        builder.Put("/produtos",
            new GatewayParameter(StandardHeaderNames.Token, ParameterType.Header, Required: false)
        );
    }
}

