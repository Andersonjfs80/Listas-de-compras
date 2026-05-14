using app_backend_produto.domain.Commands.TipoEstabelecimento.Requests;
using app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;
using Core_Logs.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace app_backend_produto.Controllers
{
	public class EstabelecimentoController : BaseController
	{
		public EstabelecimentoController(IMediator mediator) : base(mediator){}
	
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
}
