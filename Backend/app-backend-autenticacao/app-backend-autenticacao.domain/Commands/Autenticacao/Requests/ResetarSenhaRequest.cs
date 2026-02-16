using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;

namespace app_backend_autenticacao.domain.Commands.Autenticacao.Requests;

public class ResetarSenhaRequest : IRequest<ResetarSenhaResponse>
{
    public string Email { get; set; } = string.Empty;
}

