using Microsoft.AspNetCore.Mvc;
using MediatR;
using Core_Logs.Controllers;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;

namespace app_backend_autenticacao.api.Controllers;

[ApiController]
[Route("autenticacao")]
public class AutenticacaoController : BaseController
{
	public AutenticacaoController(IMediator mediator) : base(mediator){}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }

    [HttpPost("cadastrar-usuario")]
    public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }

    [HttpPost("resetar-senha")]
    public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }

    [HttpPost("cadastrar-senha")]
    public async Task<IActionResult> CadastrarSenha([FromBody] CadastrarSenhaRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }

    [HttpPost("alterar-senha")]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
    {
        var result = await _mediator.Send(request);
        return FromCommand(result);
    }
}
