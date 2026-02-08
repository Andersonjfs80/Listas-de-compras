using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Responses;

namespace app_backend_autenticacao.domain.Commands.Auth.Requests;

public class CadastrarSenhaRequest : IRequest<CadastrarSenhaResponse>
{
    public string Email { get; set; } = string.Empty;
    public string CodigoRecuperacao { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}

