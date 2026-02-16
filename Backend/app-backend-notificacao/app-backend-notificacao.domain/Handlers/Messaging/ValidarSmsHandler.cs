using MediatR;
using app_backend_notificacao.domain.Commands.Messaging.Requests;
using app_backend_notificacao.domain.Commands.Messaging.Responses;
using app_backend_notificacao.domain.Interfaces.Services;
using System.Net;

namespace app_backend_notificacao.domain.Handlers.Messaging;

public class ValidarSmsHandler(IMessagingService messagingService) : IRequestHandler<ValidarSmsRequest, ValidarSmsResponse>
{
    private readonly IMessagingService _messagingService = messagingService;

    public async Task<ValidarSmsResponse> Handle(ValidarSmsRequest request, CancellationToken cancellationToken)
    {
        var response = new ValidarSmsResponse();

        if (string.IsNullOrEmpty(request.Celular) || string.IsNullOrEmpty(request.Codigo))
        {
            return (ValidarSmsResponse)response.AdicionarErro("MSG001", "Celular e código são obrigatórios")
                                              .ComStatus(HttpStatusCode.BadRequest);
        }

        var valido = await _messagingService.ValidarCodigoAsync(request.Celular, request.Codigo, cancellationToken);

        if (!valido)
        {
            return (ValidarSmsResponse)response.AdicionarErro("MSG002", "Código inválido ou expirado")
                                              .ComStatus(HttpStatusCode.Unauthorized);
        }

        response.Sucesso = true;
        return (ValidarSmsResponse)response.ComMensagem("SMS validado com sucesso.");
    }
}

