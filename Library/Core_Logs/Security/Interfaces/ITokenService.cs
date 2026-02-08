using Core_Logs.Security.Models;
using System.Security.Claims;

namespace Core_Logs.Security.Interfaces;

/// <summary>
/// Interface universal para criação e leitura de tokens (JWT ou JOSE).
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera uma string de token a partir de uma sessão de usuário.
    /// </summary>
    string GenerateToken(UserSession session);

    /// <summary>
    /// Valida e extrai os dados do token para um objeto UserSession.
    /// </summary>
    UserSession? GetSession(string token);

    /// <summary>
    /// Extrai as Claims brutas do token.
    /// </summary>
    ClaimsPrincipal? GetPrincipal(string token);
}
