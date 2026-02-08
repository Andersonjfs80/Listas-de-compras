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
}
