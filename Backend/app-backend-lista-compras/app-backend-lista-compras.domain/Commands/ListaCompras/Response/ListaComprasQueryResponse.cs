using Core_Logs.Commands;
using app_backend_lista_compras.domain.Commands.Shared;

namespace app_backend_lista_compras.domain.Commands.ListaCompras.Response
{
	public class ListaComprasQueryResponse : BaseCommand
	{
		public List<ListaComprasResponse> Listas { get; set; } = new();
	}

	public class ListaComprasResponse
	{
		public Guid Id { get; set; }
		public string Nome { get; set; } = string.Empty;		
		public int TotalItens { get; set; }
		public int ItensMarcados { get; set; }
		public List<ItemListaResponse> Itens { get; set; } = new();
	}

	public class ItemListaResponse
	{
		public Guid Id { get; set; }
		public Guid ListaId { get; set; }
		public Guid UsuarioId { get; set; }
		public DateTime DataCadastro { get; set; }
		public string NomeProduto { get; set; } = string.Empty;
		public string NomeCurto { get; set; } = string.Empty;
		public decimal Quantidade { get; set; }
		public UnidadeMedidaModel UnidadeMedida { get; set; } = new();
		public decimal QuantidadeConversao { get; set; }	
		public UnidadeMedidaModel UnidadeMedidaConversao { get; set; } = new();	
		public bool Marcado { get; set; }
		public decimal PrecoCompra { get; set; }
		public decimal PrecoVenda { get; set; }
		
		public ProdutoResponse Produto { get; set; } = new();
	}
}
