namespace app_backend_lista_compras.domain.Models;

public class ItemListaModel
{
    public Guid Id { get; set; }
    public Guid ListaId { get; set; }
    
    // Vínculos do Produto
    public Guid? ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public string NomeCurto { get; set; } = string.Empty;
    
    // Dados de Compra
    public decimal Quantidade { get; set; } = 1;
    public string UnidadeMedida { get; set; } = "un";
    
    public decimal QuantidadeConversao { get; set; }
    public string UnidadeMedidaConversao { get; set; } = string.Empty;

    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }

    public bool Marcado { get; set; } = false;
    
    public Guid? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public string? Imagem { get; set; }
    
    public DateTime DataCadastro { get; set; }

    // Navegação
    public ListaComprasModel Lista { get; set; } = null!;
}
