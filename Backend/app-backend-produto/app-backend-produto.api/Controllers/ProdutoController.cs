using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core_Logs.Controllers;
using app_backend_produto.domain.Commands.TipoEstabelecimento.Requests;
using app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;
using app_backend_produto.domain.Commands.Produto.Requests;

namespace app_backend_produto.api.Controllers;

/// <summary>
/// Controller para operações de produtos e cadastros auxiliares
/// </summary>
[ApiController]
[Route("produtos")]
public class ProdutoController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Lista produtos com paginação e filtros
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ProdutoListagemQueryRequest request,
        CancellationToken cancellationToken)
    {
        request.Headers = ObterHeaders();
        var result = await _mediator.Send(request, cancellationToken);
        return FromCommand(result);
    }

    /// <summary>
    /// Lista todos os tipos de estabelecimento
    /// </summary>
    [HttpGet("tipos-estabelecimento")]
    public async Task<IActionResult> ListarTiposEstabelecimento(CancellationToken cancellationToken)
    {
        var request = new ListarTipoEstabelecimentoQueryRequest
        {
            Headers = ObterHeaders()
        };

        var result = await _mediator.Send(request, cancellationToken);
        return FromCommand(result);
    }
}

