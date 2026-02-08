using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Core_Logs.Controllers;
using app_backend_autenticacao.domain.Commands.Auth.Requests;
using app_backend_autenticacao.domain.Commands.Auth.Responses;

namespace app_backend_autenticacao.api.Controllers;

/// <summary>
/// Controller responsável pelos processos de autenticação e gestão de usuários.
/// </summary>
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Realiza a autenticação do usuário por e-mail, documento ou apelido.
    /// </summary>
    /// <param name="request">Dados de identificação e senha.</param>
    /// <returns>Token de acesso e refresh token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return FromCommand(await _mediator.Send(request));
    }

    /// <summary>
    /// Realiza o cadastro de um novo usuário.
    /// </summary>
    /// <param name="request">Dados básicos do usuário.</param>
    /// <returns>ID do usuário criado.</returns>
    [HttpPost("cadastrar")]
    [ProducesResponseType(typeof(CadastrarUsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarUsuarioRequest request)
    {
        return FromCommand(await _mediator.Send(request));
    }

    /// <summary>
    /// Solicita o reset de senha de um usuário.
    /// </summary>
    /// <param name="request">E-mail do usuário.</param>
    /// <returns>Status da solicitação.</returns>
    [HttpPost("resetar-senha")]
    [ProducesResponseType(typeof(ResetarSenhaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaRequest request)
    {
        return FromCommand(await _mediator.Send(request));
    }

    /// <summary>
    /// Efetivada a nova senha após o reset.
    /// </summary>
    /// <param name="request">Token de reset e nova senha.</param>
    /// <returns>Status da alteração.</returns>
    [HttpPost("cadastrar-senha")]
    [ProducesResponseType(typeof(CadastrarSenhaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarSenha([FromBody] CadastrarSenhaRequest request)
    {
        return FromCommand(await _mediator.Send(request));
    }

    /// <summary>
    /// Renova o token de acesso utilizando um Refresh Token.
    /// </summary>
    /// <param name="request">Access Token expirado e Refresh Token válido.</param>
    /// <returns>Novo par de tokens.</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return FromCommand(await _mediator.Send(request));
    }

    /// <summary>
    /// Encerra a sessão do usuário invalidando o Refresh Token.
    /// </summary>
    /// <returns>Status do logout.</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        return FromCommand(await _mediator.Send(new LogoutRequest()));
    }
}

