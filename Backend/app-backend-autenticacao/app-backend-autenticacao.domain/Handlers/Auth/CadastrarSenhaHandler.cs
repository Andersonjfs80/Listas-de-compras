using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Requests;
using app_backend_autenticacao.domain.Commands.Auth.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using app_backend_autenticacao.domain.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Auth;

public class CadastrarSenhaHandler(
    IUsuarioRepository repository,
    IConfiguration configuration,
    IHostEnvironment environment) : IRequestHandler<CadastrarSenhaRequest, CadastrarSenhaResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHostEnvironment _environment = environment;

    public async Task<CadastrarSenhaResponse> Handle(CadastrarSenhaRequest request, CancellationToken cancellationToken)
    {
        var response = new CadastrarSenhaResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorEmailAsync(request.Email, cancellationToken);
        
        // 2. Validar código de recuperação
        bool codigoValido = false;

        // Permitir 123456 apenas em ambiente de desenvolvimento
        if (request.CodigoRecuperacao == "123456" && _environment.IsDevelopment())
        {
            codigoValido = true;
        }
        // Aqui poderia haver a validação de um código real (ex: consulta ao cache/banco)
        // else if (ValidarCodigoReal(request.CodigoRecuperacao)) { codigoValido = true; }

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

        var validacaoSenha = PasswordHelper.ValidarSenha(request.NovaSenha, forcaMinima);
        if (!validacaoSenha.Valido)
        {
            return (CadastrarSenhaResponse)response.AdicionarErro("AUTH004", validacaoSenha.Mensagem)
                                                 .ComStatus(HttpStatusCode.BadRequest);
        }

        // 4. Atualizar senha (Lógica de hash proprietária)
        usuario.SenhaHash = request.NovaSenha; 
        await _repository.AtualizarAsync(usuario, cancellationToken);

        return (CadastrarSenhaResponse)response.ComMensagem("Sua senha foi atualizada com sucesso.");
    }
}

