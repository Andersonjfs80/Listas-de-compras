using app_backend_produto.domain.Models;
using Core_Logs.Commands;

namespace app_backend_produto.domain.Commands.Produto.Responses
{
	/// <summary>
	/// Response para listagem paginada de produtos (herda de BaseCommand)
	/// </summary>
	public class FiltrarProdutosCommandResponse : BaseCommand
	{
		public List<ProdutoResponse> Produtos { get; set; } = [];
		public PaginacaoInfoResponse Paginacao { get; set; } = new();
	}

	/// <summary>
	/// DTO para item de produto na listagem
	/// </summary>
	public class ProdutoResponse
	{
		public Guid Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string NomeCurto { get; set; } = string.Empty;
		public bool Ativo { get; set; }
		public ProdutoImagemModel Imagem { get; set; }
		public ProdutoCodigoModel Codigo { get; set; }
		public ProdutoCategoriaModel Categoria { get; set; }
		public TipoPrecoModel TabelaPreco { get; set; }
		public ProdutoPrecoModel Preco { get; set; }
	}
}





