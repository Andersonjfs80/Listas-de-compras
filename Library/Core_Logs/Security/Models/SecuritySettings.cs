namespace Core_Logs.Security.Models;

public enum TokenProviderType
{
    JWT,
    JOSE
}

public class SecuritySettings
{
    public const string SectionName = "Security";

    /// <summary>
    /// Define qual provedor de token usar: JWT ou JOSE
    /// </summary>
    public TokenProviderType TokenProvider { get; set; } = TokenProviderType.JWT;

    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração do Access Token em minutos (padrão 5)
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 5;

    /// <summary>
    /// Tempo de expiração do Refresh Token em minutos (padrão 5 conforme solicitado)
    /// </summary>
    public int RefreshTokenExpirationInMinutes { get; set; } = 5;

    /// <summary>
    /// Dias para expiração da senha do usuário (padrão 90 dias)
    /// </summary>
    public int PasswordExpirationDays { get; set; } = 90;

    /// <summary>
    /// Dias que antecedem a expiração para começar a exibir avisos (padrão 7 dias)
    /// </summary>
    public int PasswordWarningDays { get; set; } = 7;

    /// <summary>
    /// Habilita/Desabilita a criptografia do corpo (trânsito)
    /// </summary>
    public bool EnableBodyEncryption { get; set; } = false;

    /// <summary>
    /// Chave mestra usada para criptografia Tipo 1 (Fixo/Permanente)
    /// </summary>
    public string FixedEncryptionKey { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de vida das chaves temporárias de sessão em minutos
    /// </summary>
    public int SessionKeyExpirationMinutes { get; set; } = 15;
}
