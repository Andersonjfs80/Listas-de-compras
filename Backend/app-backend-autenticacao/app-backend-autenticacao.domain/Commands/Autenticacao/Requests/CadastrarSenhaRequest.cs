using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;

namespace app_backend_autenticacao.domain.Commands.Autenticacao.Requests;

public class CadastrarSenhaRequest : IRequest<CadastrarSenhaResponse>
{
    public string Email { get; set; } = string.Empty;
    public string CodigoRecuperacao { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}

