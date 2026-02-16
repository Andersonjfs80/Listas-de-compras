using Core_Logs.Interfaces;

namespace app_api_autenticacao.Configuration;

public class AutenticacaoSettings : GatewaySettings 
{
    public const string SectionName = "AutenticacaoSettings";
    public string UrlBackend { get; set; } = string.Empty;
}
