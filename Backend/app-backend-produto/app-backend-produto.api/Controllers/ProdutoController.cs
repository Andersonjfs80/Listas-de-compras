using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core_Logs.Controllers;
using app_backend_produto.domain.Commands.Produto.Requests;

namespace app_backend_produto.api.Controllers;

/// <summary>
/// Controller para operações de produtos
/// </summary>
[ApiController]
[Route("produtos")]
public class ProdutoController : BaseController
{
    private readonly IMediator _mediator;

    public ProdutoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista produtos com paginação e filtros
    /// </summary>
    /// <param name="request">Parâmetros de paginação e filtros</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de produtos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] ProdutoListagemQueryRequest request,
        CancellationToken cancellationToken)
    {
        // Extrai headers padronizados
        request.Headers = ObterHeaders();

        var result = await _mediator.Send(request, cancellationToken);

        return FromCommand(result);
    }
}

