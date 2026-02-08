using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Jose;

using Microsoft.Extensions.Options;

namespace Core_Logs.Security.Services;

/// <summary>
/// Implementação JOSE (JWE - Encrypted).
/// Focada em PRIVACIDADE: O conteúdo do token é criptografado e ilegível sem a chave.
/// </summary>
public class JoseTokenService : ITokenService
{
    private readonly SecuritySettings _settings;

    public JoseTokenService(IOptions<SecuritySettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(UserSession session)
    {
        var secretKey = string.IsNullOrEmpty(_settings.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : _settings.SecretKey;
        var encryptionKey = Encoding.ASCII.GetBytes(secretKey.PadRight(32).Substring(0, 32));
        var payload = new Dictionary<string, object>
        {
            { "sub", session.Id },
            { "name", session.NomeExibicao },
            { "doc", session.Documento },
            { "email", session.Email },
            { "apelido", session.Apelido },
            { "app", session.SiglaAplicacao },
            { "roles", session.Roles },
            { "claims", session.AdditionalClaims },
            { "exp", DateTimeOffset.UtcNow.AddMinutes(_settings.ExpirationInMinutes).ToUnixTimeSeconds() }
        };

        return Jose.JWT.Encode(payload, encryptionKey, JweAlgorithm.DIR, JweEncryption.A256GCM);
    }

    public UserSession? GetSession(string token)
    {
        try
        {
            var secretKey = string.IsNullOrEmpty(_settings.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : _settings.SecretKey;
            var encryptionKey = System.Text.Encoding.ASCII.GetBytes(secretKey.PadRight(32).Substring(0, 32));
            string json = Jose.JWT.Decode(token, encryptionKey);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (data == null) return null;

            var session = new UserSession
            {
                Id = data["sub"]?.ToString() ?? string.Empty,
                NomeExibicao = data["name"]?.ToString() ?? string.Empty,
                Documento = data["doc"]?.ToString() ?? string.Empty,
                Email = data["email"]?.ToString() ?? string.Empty,
                Apelido = data["apelido"]?.ToString() ?? string.Empty,
                SiglaAplicacao = data["app"]?.ToString() ?? string.Empty,
                Roles = ((JsonElement)data["roles"]).EnumerateArray().Select(x => x.GetString() ?? "").ToList()
            };

            if (data.TryGetValue("claims", out var extraClaims) && extraClaims is JsonElement claimsElement)
            {
                session.AdditionalClaims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsElement.GetRawText()) ?? new();
            }

            return session;
        }
        catch
        {
            return null;
        }
    }

    public ClaimsPrincipal? GetPrincipal(string token)
    {
        var session = GetSession(token);
        if (session == null) return null;

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

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "JOSE"));
    }
}
