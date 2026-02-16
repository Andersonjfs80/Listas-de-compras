using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Core_Logs.Constants;
using Core_Logs.Interfaces;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Autenticacao;

public class LoginHandler(
    IUsuarioRepository repository,
    ITokenService tokenService,
    ISecurityService securityService,
    ICacheService cacheService,
    IHttpContextAccessor httpContextAccessor,
    IOptions<SecuritySettings> securitySettings) : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ISecurityService _securityService = securityService;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = new LoginResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorIdentificadorAsync(request.Identificador, cancellationToken);
        
        // 2. Validar existência e senha com o novo motor de segurança
        if (usuario == null || !_securityService.VerifyPassword(request.Senha, usuario.SenhaHash))
        {
            return (LoginResponse)response.AdicionarErro("AUTH001", "Identificador ou senha inválidos")
                                        .ComStatus(HttpStatusCode.Unauthorized);
        }

        // 2.1 Verificar status de expiração de senha
        var passwordStatus = _securityService.GetPasswordStatus(usuario.DataAtualizacaoSenha ?? usuario.DataCriacao);
        
        if (passwordStatus == PasswordStatus.Expired)
        {
            return (LoginResponse)response.AdicionarErro("AUTH003", "Sua senha expirou e sua conta está bloqueada. Por favor, realize a troca obrigatória.")
                                        .ComStatus(HttpStatusCode.Forbidden);
        }
        
        if (passwordStatus == PasswordStatus.Warning)
        {
            var daysRemaining = _securityService.GetDaysToPasswordExpire(usuario.DataAtualizacaoSenha ?? usuario.DataCriacao);
            response.AdicionarWarning("AUTH004", $"Sua senha expirará em {daysRemaining} dias. Recomendamos a troca imediata.");
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

        // 4.5 Persistir Sessão no Redis (Assinatura de Segurança)
        var sessionId = _httpContextAccessor.HttpContext?.Request.Headers[StandardHeaderNames.SessionId].ToString();
        var hardwareId = _httpContextAccessor.HttpContext?.Request.Headers[StandardHeaderNames.HardwareId].ToString();

        if (!string.IsNullOrEmpty(sessionId))
        {
            var cacheKey = $"Auth:Session:{sessionId}";
            var sessionData = new
            {
                User = userSession,
                HardwareId = hardwareId,
                LoginTime = DateTime.UtcNow,
                Token = tokenResult
            };

            await _cacheService.SetAsync(cacheKey, sessionData, 
                TimeSpan.FromMinutes(_securitySettings.ExpirationInMinutes), cancellationToken);
        }

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

