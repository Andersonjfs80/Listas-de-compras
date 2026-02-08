using MediatR;
using app_backend_seguranca.domain.Commands.Messaging.Responses;

namespace app_backend_seguranca.domain.Commands.Messaging.Requests;

public class ValidarSmsRequest : IRequest<ValidarSmsResponse>
{
    public string Celular { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
}

