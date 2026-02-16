using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;

namespace app_backend_autenticacao.domain.Commands.Autenticacao.Requests;

public class RefreshTokenRequest : IRequest<RefreshTokenResponse>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

