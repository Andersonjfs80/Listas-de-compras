namespace app_backend_produto.domain.Models;

public class ProdutoModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeCurto { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    // FK
    public Guid CategoriaId { get; set; }
    public CategoriaModel Categoria { get; set; } = null!;
    
    public Guid UsuarioId { get; set; }
    
    // Navegação
    public ICollection<PrecoModel> Precos { get; set; } = new List<PrecoModel>();
    public ICollection<CodigoProdutoModel> Codigos { get; set; } = new List<CodigoProdutoModel>();
}

