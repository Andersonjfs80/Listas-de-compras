using app_backend_produto.domain.Commands.UnidadeMedida.Requests;
using Core_Logs.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace app_backend_produto.api.Controllers;

/// <summary>
/// Controller para operações de Unidades de Medida
/// </summary>
[ApiController]
[Route("unidades-medida")]
public class UnidadeMedidaController : BaseController
{
    public UnidadeMedidaController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Lista todas as unidades de medida ativas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var request = new ListarUnidadesMedidaQueryRequest
        {
            Headers = ObterHeaders()
        };

        var result = await _mediator.Send(request, cancellationToken);
        return FromCommand(result);
    }
}
