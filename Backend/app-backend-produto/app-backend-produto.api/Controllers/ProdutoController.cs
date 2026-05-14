using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core_Logs.Controllers;
using app_backend_produto.domain.Commands.Produto.Requests;

namespace app_backend_produto.api.Controllers;

/// <summary>
/// Controller para operações de produtos e cadastros auxiliares
/// </summary>
[ApiController]
[Route("produtos")]
public class ProdutoController : BaseController
{
	public ProdutoController(IMediator mediator) : base(mediator) {}

	/// <summary>
	/// Busca / Lista produtos com parâmetros no corpo da requisição (POST)
	/// </summary>
	[HttpPost("filtrar")]
    public async Task<IActionResult> Filtrar(
        [FromBody] FiltrarProdutosCommandRequest request,
        CancellationToken cancellationToken)
    {
        request.Headers = ObterHeaders();
        var result = await _mediator.Send(request, cancellationToken);
        return FromCommand(result);
    }

    /// <summary>
    /// Adiciona um novo produto
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Adicionar(
        [FromBody] AdicionarProdutoCommandRequest request,
        CancellationToken cancellationToken)
    {
        request.Headers = ObterHeaders();
        var result = await _mediator.Send(request, cancellationToken);
        return FromCommand(result);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Atualizar(
        [FromBody] AtualizarProdutoCommandRequest request,
        CancellationToken cancellationToken)
    {
        request.Headers = ObterHeaders();
        var result = await _mediator.Send(request, cancellationToken);
        return FromCommand(result);
    }

}

