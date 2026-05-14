using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using MediatR;
using System.Text.Json.Serialization;
using Core_Logs.Models.Request;

namespace app_backend_lista_compras.domain.Commands.ListaCompras.Request
{
	public class ListaComprasQueryRequest : IRequest<ListaComprasQueryResponse>
	{
		/// <summary>
		/// Headers da requisição (obtidos do BaseController)
		/// </summary>
		[JsonIgnore]
		public RequestHeaders Headers { get; set; } = new();
	}
}
