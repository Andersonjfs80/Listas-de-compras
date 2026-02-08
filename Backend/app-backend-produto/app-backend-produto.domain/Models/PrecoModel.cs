namespace app_backend_produto.domain.Models;

public class PrecoModel
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public bool Principal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    // FK
    public Guid ProdutoId { get; set; }
    public ProdutoModel Produto { get; set; } = null!;
    
    public Guid TipoPrecoId { get; set; }
    public TipoPrecoModel TipoPreco { get; set; } = null!;
    
    public Guid UsuarioId { get; set; }
}

