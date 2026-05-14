using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using app_backend_autenticacao.domain.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Core_Logs.Security.Models;
using Core_Logs.Interfaces;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Autenticacao;

public class AlterarSenhaHandler(
    IUsuarioRepository repository,
    IConfiguration configuration,
    Core_Logs.Interfaces.ISecurityService securityService,
    IOptions<SecuritySettings> securityOptions) : IRequestHandler<AlterarSenhaRequest, AlterarSenhaResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly IConfiguration _configuration = configuration;
    private readonly Core_Logs.Interfaces.ISecurityService _securityService = securityService;
    private readonly SecuritySettings _securitySettings = securityOptions.Value;

    public async Task<AlterarSenhaResponse> Handle(AlterarSenhaRequest request, CancellationToken cancellationToken)
    {
        var response = new AlterarSenhaResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorEmailAsync(request.Email, cancellationToken);
        if (usuario == null)
        {
            return (AlterarSenhaResponse)response.AdicionarErro("AUTH005", "Usuário não encontrado.")
                                                 .ComStatus(HttpStatusCode.NotFound);
        }

        // 2. Validar Senha Atual
        if (!_securityService.VerifyPassword(request.SenhaAtual, usuario.SenhaHash))
        {
            return (AlterarSenhaResponse)response.AdicionarErro("AUTH007", "A senha atual informada está incorreta.")
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 3. Validar força da nova senha
        var forcaMinimaConfig = _configuration.GetValue<string>("AuthSettings:ForcaSenhaMinima") ?? "Fraca";
        if (!Enum.TryParse<ForcaSenha>(forcaMinimaConfig, true, out var forcaMinima))
        {
            forcaMinima = ForcaSenha.Fraca;
        }

        var validacaoSenha = PasswordHelper.ValidarSenha(request.NovaSenha, forcaMinima);
        if (!validacaoSenha.Valido)
        {
            return (AlterarSenhaResponse)response.AdicionarErro("AUTH004", validacaoSenha.Mensagem)
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 4. Validar histórico de senhas (não pode repetir as últimas 5)
        if (!_securityService.ValidatePasswordHistory(request.NovaSenha, usuario.HistoricoSenhasJson))
        {
            return (AlterarSenhaResponse)response.AdicionarErro("AUTH006", "Esta senha já foi utilizada recentemente e não pode ser repetida.")
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 5. Atualizar senha com Hash e Histórico
        var novaSenhaHash = _securityService.HashPassword(request.NovaSenha);
        usuario.SenhaHash = novaSenhaHash;
        usuario.DataAtualizacaoSenha = DateTime.UtcNow;
        usuario.HistoricoSenhasJson = _securityService.AddToPasswordHistory(novaSenhaHash, usuario.HistoricoSenhasJson, _securitySettings.PasswordHistoryLimit);

        await _repository.AtualizarAsync(usuario, cancellationToken);

        return (AlterarSenhaResponse)response.ComMensagem("Sua senha foi alterada com sucesso.");
    }
}
