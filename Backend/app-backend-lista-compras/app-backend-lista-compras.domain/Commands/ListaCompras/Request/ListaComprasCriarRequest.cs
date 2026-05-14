using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using MediatR;
using System.Text.Json.Serialization;
using Core_Logs.Models.Request;

namespace app_backend_lista_compras.domain.Commands.ListaCompras.Request
{
	public class ListaComprasCriarRequest : IRequest<ListaComprasCriarResponse>
	{
		/// <summary>
		/// Headers da requisição (obtidos do BaseController)
		/// </summary>
		[JsonIgnore]
		public RequestHeaders Headers { get; set; } = new();
		
		public string Nome { get; set; } = string.Empty;
	}
}
