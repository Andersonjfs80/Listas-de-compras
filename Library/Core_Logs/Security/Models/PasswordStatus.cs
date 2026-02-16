namespace Core_Logs.Security.Models;

/// <summary>
/// Define os possíveis estados de expiração de uma senha.
/// </summary>
public enum PasswordStatus
{
    /// <summary>
    /// Senha válida e fora do período de aviso.
    /// </summary>
    Valid,

    /// <summary>
    /// Senha próxima do vencimento (dentro da janela de aviso).
    /// </summary>
    Warning,

    /// <summary>
    /// Senha expirada. O login deve ser bloqueado para troca.
    /// </summary>
    Expired
}
