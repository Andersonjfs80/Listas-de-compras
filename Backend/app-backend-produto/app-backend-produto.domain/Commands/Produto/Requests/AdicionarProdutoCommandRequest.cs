

using app_backend_produto.domain.Commands.Produto.Responses;
using Core_Logs.Models.Request;
using MediatR;
using System.Text.Json.Serialization;

namespace app_backend_produto.domain.Commands.Produto.Requests
{
	/// <summary>
	/// Request para adicionar um novo produto
	/// </summary>
	public class AdicionarProdutoCommandRequest : IRequest<AdicionarProdutoCommandResponse>
	{
		/// <summary>
		/// Headers da requisição (obtidos do BaseController)
		/// </summary>
		[JsonIgnore]
		public RequestHeaders Headers { get; set; } = new();

		public string Nome { get; set; } = string.Empty;
		public string NomeCurto { get; set; } = string.Empty;
		public bool Ativo { get; set; } = true;
	}
}
