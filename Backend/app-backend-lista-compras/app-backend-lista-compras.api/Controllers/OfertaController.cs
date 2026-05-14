using app_backend_lista_compras.domain.Commands.Oferta.Request;
using Core_Logs.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace app_backend_lista_compras.Controllers;

[ApiController]
[Route("ofertas")]
public class OfertaController : BaseController
{
    public OfertaController(IMediator mediator) : base(mediator) { }

	/// <summary>Retorna ofertas ativas paginadas (POST com corpo)</summary>
	[HttpPost("filtrar")]
    public async Task<IActionResult> Filtrar(
        [FromBody] FiltrarOfertasCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Headers = ObterHeaders();
        var response = await _mediator.Send(request, cancellationToken);
        return FromCommand(response);
    }
}
