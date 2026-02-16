namespace app_backend_produto.domain.Models;

public class CategoriaModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    // FK - Auto-relacionamento (Categoria Pai)
    // FK - Auto-relacionamento (Categoria Pai)
    public Guid? OwnerId { get; set; }
    public CategoriaModel? Owner { get; set; }

    public Guid UsuarioId { get; set; }
    
    // Navegação
    public ICollection<CategoriaModel> SubCategorias { get; set; } = new List<CategoriaModel>();
    public ICollection<ProdutoCategoriaModel> ProdutoCategorias { get; set; } = new List<ProdutoCategoriaModel>();
}

