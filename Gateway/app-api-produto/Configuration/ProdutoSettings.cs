using Core_Logs.Interfaces;

namespace app_api_produto.Configuration;

public class ProdutoSettings : GatewaySettings
{
    public const string SectionName = "ProdutoSettings";
    
    // Tags Globais (Raiz do appsettings)
    public string AppName { get; set; } = string.Empty;
    public string PathBase { get; set; } = string.Empty;

    // Configurações Específicas
    public string UrlBackend { get; set; } = string.Empty;
}

