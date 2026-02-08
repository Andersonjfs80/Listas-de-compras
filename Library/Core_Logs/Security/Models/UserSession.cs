namespace Core_Logs.Security.Models;

/// <summary>
/// Representa os dados básicos de um usuário extraídos do Token.
/// </summary>
public class UserSession
{
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome formatado para exibição (ex: "Anderson Silva" ou "Anderson").
    /// </summary>
    public string NomeExibicao { get; set; } = string.Empty;

    /// <summary>
    /// Documento de identificação (CPF, CNPJ, etc).
    /// </summary>
    public string Documento { get; set; } = string.Empty;

    public string Apelido { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string SiglaAplicacao { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    
    /// <summary>
    /// Claims customizadas (ex: "Filial", "LimiteAprovacao", etc).
    /// </summary>
    public Dictionary<string, string> AdditionalClaims { get; set; } = new();
}
