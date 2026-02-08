using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Requests;
using app_backend_autenticacao.domain.Commands.Auth.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Options;

namespace app_backend_autenticacao.domain.Handlers.Auth;

public class RefreshTokenHandler(
    IUsuarioRepository repository,
    ITokenService tokenService,
    IOptions<SecuritySettings> securitySettings) : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = new RefreshTokenResponse();

        // 1. Extrair Claims do token expirado
        var principal = ObterPrincipalDoTokenExpirado(request.AccessToken);
        if (principal == null)
        {
            return (RefreshTokenResponse)response.AdicionarErro("REF001", "Token de acesso inválido")
                                               .ComStatus(HttpStatusCode.BadRequest);
        }

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return (RefreshTokenResponse)response.AdicionarErro("REF002", "Token de acesso sem informações de usuário")
                                               .ComStatus(HttpStatusCode.BadRequest);
        }

        // 2. Buscar usuário
        var usuario = await _repository.ObterPorEmailAsync(email, cancellationToken);
        if (usuario == null || usuario.RefreshToken != request.RefreshToken || usuario.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return (RefreshTokenResponse)response.AdicionarErro("REF003", "Refresh Token inválido ou expirado")
                                               .ComStatus(HttpStatusCode.Unauthorized);
        }

        // 3. Gerar novos tokens
        var userSession = new UserSession
        {
            Id = usuario.Id.ToString(),
            NomeExibicao = usuario.Nome,
            Email = usuario.Email
        };

        var novoAccessToken = _tokenService.GenerateToken(userSession);
        var novoRefreshToken = Guid.NewGuid().ToString().Replace("-", "");

        // 4. Atualizar no banco
        usuario.RefreshToken = novoRefreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_securitySettings.RefreshTokenExpirationInMinutes);
        await _repository.AtualizarAsync(usuario, cancellationToken);

        // 5. Retornar
        response.AccessToken = novoAccessToken;
        response.RefreshToken = novoRefreshToken;

        return (RefreshTokenResponse)response.ComMensagem("Tokens renovados com sucesso");
    }

    private ClaimsPrincipal? ObterPrincipalDoTokenExpirado(string token)
    {
        // Nota: Idealmente esta lógica deveria estar no ITokenService, 
        // mas como não está, implementamos aqui para extrair claims sem validar expiração.
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "Bearer"));
        }
        catch
        {
            return null;
        }
    }
}

