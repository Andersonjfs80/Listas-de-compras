using Core_Logs.Interfaces;

namespace app_api_produto.Configuration;

public class ProdutoSettings : GatewaySettings
{
    public const string SectionName = "ProdutoSettings";
    
    // Configurações Específicas
    public string UrlBackend { get; set; } = string.Empty;
}

