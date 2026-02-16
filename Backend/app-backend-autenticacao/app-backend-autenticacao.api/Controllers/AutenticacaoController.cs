using Microsoft.AspNetCore.Mvc;
using MediatR;
using Core_Logs.Controllers;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;

namespace app_backend_autenticacao.api.Controllers;

[ApiController]
[Route("autenticacao")]
public class AutenticacaoController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }
}
