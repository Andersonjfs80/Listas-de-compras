using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Requests;
using app_backend_autenticacao.domain.Commands.Auth.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;

namespace app_backend_autenticacao.domain.Handlers.Auth;

public class ResetarSenhaHandler(
    IUsuarioRepository repository) : IRequestHandler<ResetarSenhaRequest, ResetarSenhaResponse>
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<ResetarSenhaResponse> Handle(ResetarSenhaRequest request, CancellationToken cancellationToken)
    {
        var response = new ResetarSenhaResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorEmailAsync(request.Email, cancellationToken);
        
        // 2. Lógica de "blind resets" (não informa se o e-mail existe)
        if (usuario != null)
        {
             // Logica para gerar código de recuperação e enviar e-mail
             // usuario.CodigoRecuperacao = "123456"; 
             // await _repository.AtualizarAsync(usuario, cancellationToken);
        }

        return (ResetarSenhaResponse)response.ComMensagem("Se o e-mail existir, um código de recuperação foi enviado.");
    }
}

