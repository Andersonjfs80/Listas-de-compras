using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core_Logs.Controllers;
using app_backend_notificacao.domain.Commands.Messaging.Requests;

namespace app_backend_notificacao.api.Controllers;

[ApiController]
[Route("notificacoes")]
public class NotificacaoController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("validar-sms")]
    public async Task<IActionResult> ValidarSms([FromBody] ValidarSmsRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }
}

