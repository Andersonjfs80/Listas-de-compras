using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using app_backend_autenticacao.domain.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Core_Logs.Security.Models;
using Core_Logs.Interfaces;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Autenticacao;

public class CadastrarSenhaHandler(
    IUsuarioRepository repository,
    IConfiguration configuration,
    IHostEnvironment environment,
    Core_Logs.Interfaces.ISecurityService securityService,
    IOptions<SecuritySettings> securityOptions,
    ICacheService cacheService) : IRequestHandler<CadastrarSenhaRequest, CadastrarSenhaResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHostEnvironment _environment = environment;
    private readonly Core_Logs.Interfaces.ISecurityService _securityService = securityService;
    private readonly SecuritySettings _securitySettings = securityOptions.Value;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<CadastrarSenhaResponse> Handle(CadastrarSenhaRequest request, CancellationToken cancellationToken)
    {
        var response = new CadastrarSenhaResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorEmailAsync(request.Email, cancellationToken);
        
        // 2. Validar código de recuperação
        bool codigoValido = false;

        // 2.1 Permitir 123456 apenas em ambiente de desenvolvimento
        if (request.CodigoRecuperacao == "123456" && _environment.IsDevelopment())
        {
            codigoValido = true;
        }
        else
        {
            // 2.2 Validar código real no cache
            var cacheKey = $"Auth:ResetCode:{request.Email}";
            var codigoSalvo = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);
            
            if (codigoSalvo != null && codigoSalvo == request.CodigoRecuperacao)
            {
                codigoValido = true;
                // Opcional: Remover do cache após uso único
                await _cacheService.RemoveAsync(cacheKey, cancellationToken);
            }
        }

        if (usuario == null || !codigoValido)
        {
            return (CadastrarSenhaResponse)response.AdicionarErro("AUTH003", "Código de recuperação inválido ou expirado")
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 3. Validar força da senha
        var forcaMinimaConfig = _configuration.GetValue<string>("AuthSettings:ForcaSenhaMinima") ?? "Fraca";
        if (!Enum.TryParse<ForcaSenha>(forcaMinimaConfig, true, out var forcaMinima))
        {
            forcaMinima = ForcaSenha.Fraca;
        }

        var novaSenhaOriginal = _securityService.DecryptFixed(request.NovaSenha);
        var validacaoSenha = PasswordHelper.ValidarSenha(novaSenhaOriginal, forcaMinima);
        if (!validacaoSenha.Valido)
        {
            return (CadastrarSenhaResponse)response.AdicionarErro("AUTH004", validacaoSenha.Mensagem)
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 4. Validar histórico de senhas
        if (!_securityService.ValidatePasswordHistory(novaSenhaOriginal, usuario.HistoricoSenhasJson))
        {
            return (CadastrarSenhaResponse)response.AdicionarErro("AUTH006", "Esta senha já foi utilizada recentemente e não pode ser repetida.")
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 5. Atualizar senha com Hash e Histórico
        var novaSenhaHash = _securityService.HashPassword(novaSenhaOriginal);
        usuario.SenhaHash = novaSenhaHash;
        usuario.DataAtualizacaoSenha = DateTime.UtcNow;
        usuario.HistoricoSenhasJson = _securityService.AddToPasswordHistory(novaSenhaHash, usuario.HistoricoSenhasJson, _securitySettings.PasswordHistoryLimit);

        await _repository.AtualizarAsync(usuario, cancellationToken);

        return (CadastrarSenhaResponse)response.ComMensagem("Sua senha foi atualizada com sucesso.");
    }
}

