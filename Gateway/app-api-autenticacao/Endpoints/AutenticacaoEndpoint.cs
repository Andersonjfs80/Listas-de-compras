using Core_Logs.Extensions;
using Core_Http.Gateway;
using app_api_autenticacao.Configuration;

namespace app_api_autenticacao.Endpoints;

public class AutenticacaoEndpoint : BaseGatewayEndpoint<AutenticacaoSettings>
{
    public override string ServiceName => "AutenticacaoBackend";
    public override Func<AutenticacaoSettings, string> UrlSelector => settings => settings.UrlBackend;

    public override void Configure(GatewayBuilder builder)
    {
        // Login e Cadastro
        builder.Post("/autenticacao/login");
            
        builder.Post("/autenticacao/cadastrar",
            new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header));

        // Gestão de Senha
        builder.Post("/autenticacao/resetar-senha",
            new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header));
            
        builder.Post("/autenticacao/cadastrar-senha",
            new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header));

        // Sessão
        builder.Post("/autenticacao/refresh-token",
            new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header));

        builder.Post("/autenticacao/logout",
             new GatewayParameter(Core_Logs.Constants.StandardHeaderNames.Token, ParameterType.Header)); 
    }
}
