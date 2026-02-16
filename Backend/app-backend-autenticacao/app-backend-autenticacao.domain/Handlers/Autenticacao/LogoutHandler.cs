using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;
using Core_Logs.Security.Interfaces;
using app_backend_autenticacao.domain.Interfaces.Repositories;

namespace app_backend_autenticacao.domain.Handlers.Autenticacao;

public class LogoutHandler(
    IUsuarioRepository repository,
    IUserContext userContext) : IRequestHandler<LogoutRequest, LogoutResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly IUserContext _userContext = userContext;

    public async Task<LogoutResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var response = new LogoutResponse();
        var email = _userContext.Email;

        if (!string.IsNullOrEmpty(email))
        {
            var usuario = await _repository.ObterPorEmailAsync(email, cancellationToken);
            if (usuario != null)
            {
                usuario.RefreshToken = null;
                usuario.RefreshTokenExpiryTime = null;
                await _repository.AtualizarAsync(usuario, cancellationToken);
            }
        }

        return (LogoutResponse)response.ComMensagem("Logout realizado com sucesso");
    }
}

