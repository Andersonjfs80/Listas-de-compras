using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Responses;

namespace app_backend_autenticacao.domain.Commands.Auth.Requests;

public class LogoutRequest : IRequest<LogoutResponse>
{
}

