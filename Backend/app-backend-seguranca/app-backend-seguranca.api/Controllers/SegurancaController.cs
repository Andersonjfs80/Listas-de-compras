using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core_Logs.Controllers;
using app_backend_seguranca.domain.Commands.Messaging.Requests;

namespace app_backend_seguranca.api.Controllers;

[ApiController]
[Route("api/seguranca")]
public class SegurancaController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("validar-sms")]
    public async Task<IActionResult> ValidarSms([FromBody] ValidarSmsRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }
}

