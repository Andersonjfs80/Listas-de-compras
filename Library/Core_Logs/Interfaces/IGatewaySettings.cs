using System.Collections.Generic;

namespace Core_Logs.Interfaces;

public class GatewayHeaderConfig
{
    public string Name { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
}

/// <summary>
/// Interface de configurações de Gateway.
/// Movida de Core_Http para evitar dependência circular.
/// </summary>
public interface IGatewaySettings
{
    /// <summary>
    /// Nome do Artefato/Aplicação (Ex: apl-back-dg)
    /// </summary>
    string AppName { get; }

    /// <summary>
    /// Prefixo da API (Ex: /apl-back-dg)
    /// </summary>
    string PathBase { get; }
    
    /// <summary>
    /// Headers globais que devem ser trafegados/validados.
    /// </summary>
    List<GatewayHeaderConfig> GlobalHeaders { get; }
}

/// <summary>
/// Classe base para facilitar a implementação de configurações de Gateway.
/// </summary>
public abstract class GatewaySettings : IGatewaySettings
{
    public string AppName { get; set; } = string.Empty;
    public string PathBase { get; set; } = string.Empty;
    public List<GatewayHeaderConfig> GlobalHeaders { get; set; } = new();
}
