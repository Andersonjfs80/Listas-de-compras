using MediatR;
using app_backend_autenticacao.domain.Commands.Auth.Responses;

namespace app_backend_autenticacao.domain.Commands.Auth.Requests;

public class LoginRequest : IRequest<LoginResponse>
{
    /// <summary>
    /// E-mail, CPF/CNPJ ou Apelido
    /// </summary>
    public string Identificador { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

