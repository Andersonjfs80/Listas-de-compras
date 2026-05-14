using app_backend_lista_compras.domain.Commands.Oferta.Response;
using MediatR;
using System.Text.Json.Serialization;
using Core_Logs.Models.Request;


namespace app_backend_lista_compras.domain.Commands.Oferta.Request
{
    public class FiltrarOfertasCommandRequest : IRequest<FiltrarOfertasCommandResponse>
    {
        /// <summary>
		/// Headers da requisição (obtidos do BaseController)
		/// </summary>
		[JsonIgnore]
		public RequestHeaders Headers { get; set; } = new();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
