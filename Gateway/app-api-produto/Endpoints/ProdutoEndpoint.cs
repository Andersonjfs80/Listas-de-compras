using Core_Logs.Extensions;
using Core_Http.Gateway;
using app_api_produto.Configuration;

namespace app_api_produto.Endpoints;

public class ProdutoEndpoint : BaseGatewayEndpoint<ProdutoSettings>
{
    public override string ServiceName => "ProdutoBackend";
    public override Func<ProdutoSettings, string> UrlSelector => settings => settings.UrlBackend;

    public override void Configure(GatewayBuilder builder)
    {
        builder.Get("/produtos",         
            new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header),
            new GatewayParameter("pageNumber", ParameterType.Query),
            new GatewayParameter("pageSize", ParameterType.Query),
            new GatewayParameter("nome", ParameterType.Query),
            new GatewayParameter("categoriaId", ParameterType.Query),
            new GatewayParameter("ativo", ParameterType.Query),
            new GatewayParameter("ordenarPor", ParameterType.Query),
            new GatewayParameter("ordemCrescente", ParameterType.Query)
        );
    }
}

