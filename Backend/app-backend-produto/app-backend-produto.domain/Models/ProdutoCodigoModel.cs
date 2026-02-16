namespace app_backend_produto.domain.Models;

public class ProdutoCodigoModel
{
    public Guid Id { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    // FK
    public Guid ProdutoId { get; set; }
    public ProdutoModel Produto { get; set; } = null!;
    
    public Guid UnidadeMedidaId { get; set; }
    public UnidadeMedidaModel UnidadeMedida { get; set; } = null!;

    public Guid FornecedorId { get; set; }
    public FornecedorModel Fornecedor { get; set; } = null!;
    
    public Guid UsuarioId { get; set; }
}

