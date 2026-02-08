using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Requests;
using app_backend_autenticacao.domain.Commands.Auth.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Auth;

public class LoginHandler(
    IUsuarioRepository repository,
    ITokenService tokenService,
    IOptions<SecuritySettings> securitySettings) : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = new LoginResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorIdentificadorAsync(request.Identificador, cancellationToken);
        
        // 2. Validar existência e senha (Lógica simplificada para exemplo, ideal usar BCrypt)
        if (usuario == null || usuario.SenhaHash != request.Senha) // Aqui entrará a lógica de hash propriamente dita
        {
            return (LoginResponse)response.AdicionarErro("AUTH001", "Identificador ou senha inválidos")
                                        .ComStatus(HttpStatusCode.Unauthorized);
        }

        if (!usuario.Ativo)
        {
            return (LoginResponse)response.AdicionarErro("AUTH002", "Usuário inativo")
                                        .ComStatus(HttpStatusCode.Forbidden);
        }

        // 3. Gerar Token
        var userSession = new UserSession
        {
            Id = usuario.Id.ToString(),
            NomeExibicao = usuario.Nome,
            Email = usuario.Email,
            Documento = usuario.Documento,
            Apelido = usuario.Apelido
        };

        var tokenResult = _tokenService.GenerateToken(userSession);

        // 4. Gerar e Salvar Refresh Token
        var refreshToken = Guid.NewGuid().ToString().Replace("-", "");
        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_securitySettings.RefreshTokenExpirationInMinutes);
        await _repository.AtualizarAsync(usuario, cancellationToken);

        // 5. Montar Response
        response.Token = tokenResult;
        response.RefreshToken = refreshToken;
        response.ExpiresIn = DateTime.UtcNow.AddMinutes(_securitySettings.ExpirationInMinutes);
        response.Usuario = new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
        
        return (LoginResponse)response.ComMensagem("Login realizado com sucesso");
    }
}

