using Core_Logs.Commands;

namespace app_backend_produto.domain.Commands.Produto.Responses
{
	/// <summary>
	/// Response para a criação de um novo produto
	/// </summary>
	public class AdicionarProdutoCommandResponse : BaseCommand
	{
		/// <summary>
		/// ID do produto criado
		/// </summary>
		public Guid? Id { get; set; }
	}
}
