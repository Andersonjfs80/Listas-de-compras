using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;

namespace app_backend_autenticacao.domain.Commands.Autenticacao.Requests;

public class CadastrarUsuarioRequest : IRequest<CadastrarUsuarioResponse>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Apelido { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

