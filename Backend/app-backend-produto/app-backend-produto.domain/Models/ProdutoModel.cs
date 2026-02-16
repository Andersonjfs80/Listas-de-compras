namespace app_backend_produto.domain.Models;

public class ProdutoModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeCurto { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    // Navegação N-N
    public ICollection<ProdutoCategoriaModel> ProdutoCategorias { get; set; } = new List<ProdutoCategoriaModel>();
    
    public Guid UsuarioId { get; set; }
    
    // Navegação
    public ICollection<ProdutoPrecoModel> ProdutoPrecos { get; set; } = new List<ProdutoPrecoModel>();
    public ICollection<ProdutoCodigoModel> ProdutoCodigos { get; set; } = new List<ProdutoCodigoModel>();
    public ICollection<ProdutoImagemModel> ProdutoImagens { get; set; } = new List<ProdutoImagemModel>();
}

