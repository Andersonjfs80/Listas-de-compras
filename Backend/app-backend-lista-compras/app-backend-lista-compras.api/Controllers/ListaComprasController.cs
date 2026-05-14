using MediatR;
using Microsoft.AspNetCore.Mvc;
using app_backend_lista_compras.domain.Commands.ListaCompras.Request;
using Core_Logs.Controllers;

namespace app_backend_lista_compras.Controllers;

[ApiController]
[Route("listas")]
public class ListaComprasController : BaseController
{

    public ListaComprasController(IMediator mediator) : base(mediator) { }

	/// <summary>Retorna todas as listas de compras do usuário</summary>
	[HttpGet]
    public async Task<IActionResult> ObterListas(CancellationToken cancellationToken)
    {
        var request = new ListaComprasQueryRequest { Headers = ObterHeaders() };
        var response = await _mediator.Send(request, cancellationToken);
        return FromCommand(response);
    }

    /// <summary>Cria uma nova lista de compras</summary>
    [HttpPost]
    public async Task<IActionResult> CriarLista(
        [FromBody] ListaComprasCriarRequest request,
        CancellationToken cancellationToken)
    {
        request.Headers = ObterHeaders();
        var response = await _mediator.Send(request, cancellationToken);
        return FromCommand(response);
    }

    /// <summary>Adiciona um item à lista</summary>
    [HttpPost("{listaId}/itens")]
    public async Task<IActionResult> AdicionarItem(
        Guid listaId,
        [FromBody] ItemListaAdicionarRequest request,
        CancellationToken cancellationToken)
    {
        request.Headers = ObterHeaders();
        request.Item.ListaId = listaId;
        var response = await _mediator.Send(request, cancellationToken);
        return FromCommand(response);
    }

    /// <summary>Marca ou desmarca um item da lista</summary>
    [HttpPut("{listaId}/itens/{itemId}/toggle")]
    public async Task<IActionResult> ToggleItem(
        Guid listaId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var request = new ItemListaToggleRequest { ItemId = itemId, Headers = ObterHeaders() };
        var response = await _mediator.Send(request, cancellationToken);
        return FromCommand(response);
    }

    /// <summary>Remove um item da lista</summary>
    [HttpDelete("{listaId}/itens/{itemId}")]
    public async Task<IActionResult> RemoverItem(
        Guid listaId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var request = new ItemListaRemoverRequest { ItemId = itemId, Headers = ObterHeaders() };
        var response = await _mediator.Send(request, cancellationToken);
        return FromCommand(response);
    }
}
