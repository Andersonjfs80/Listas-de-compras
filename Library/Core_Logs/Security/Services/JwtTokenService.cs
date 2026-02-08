using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;

namespace Core_Logs.Security.Services;

/// <summary>
/// Implementação padrão JWT (JWS).
/// Focada em integridade: fácil de ler (Base64), impossível de alterar sem a chave secreta.
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly SecuritySettings _settings;

    public JwtTokenService(IOptions<SecuritySettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(UserSession session)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = string.IsNullOrEmpty(_settings.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : _settings.SecretKey;
        var key = Encoding.ASCII.GetBytes(secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, session.Id),
            new Claim(ClaimTypes.Name, session.NomeExibicao),
            new Claim("Documento", session.Documento),
            new Claim(ClaimTypes.Email, session.Email),
            new Claim("Apelido", session.Apelido),
            new Claim("AppSigla", session.SiglaAplicacao)
        };

        foreach (var role in session.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var claim in session.AdditionalClaims)
            claims.Add(new Claim(claim.Key, claim.Value));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public UserSession? GetSession(string token)
    {
        var principal = GetPrincipal(token);
        if (principal == null) return null;

        var session = new UserSession
        {
            Id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            NomeExibicao = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Documento = principal.FindFirst("Documento")?.Value ?? string.Empty,
            Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Apelido = principal.FindFirst("Apelido")?.Value ?? string.Empty,
            SiglaAplicacao = principal.FindFirst("AppSigla")?.Value ?? string.Empty,
            Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };

        // Recuperar claims adicionais que não mapeamos em propriedades fixas
        var fixedClaims = new[] { ClaimTypes.NameIdentifier, ClaimTypes.Name, "Documento", ClaimTypes.Email, "Apelido", "AppSigla", ClaimTypes.Role };
        foreach (var claim in principal.Claims)
        {
            if (!fixedClaims.Contains(claim.Type))
            {
                session.AdditionalClaims[claim.Type] = claim.Value;
            }
        }

        return session;
    }

    public ClaimsPrincipal? GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = string.IsNullOrEmpty(_settings.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : _settings.SecretKey;
            var key = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
