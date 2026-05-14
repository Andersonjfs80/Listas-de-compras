using Core_Logs.Interfaces;

namespace app_api_cadastro.Configuration;

public class ConfigurationSettings : GatewaySettings
{
    public const string SectionName = "ConfigurationSettings";
    
    // Configurações Específicas
    public string UrlBackendProduto { get; set; } = string.Empty;
    public string UrlBackendListaCompras { get; set; } = string.Empty;
    public string UrlBackendOferta { get; set; } = string.Empty;
}

