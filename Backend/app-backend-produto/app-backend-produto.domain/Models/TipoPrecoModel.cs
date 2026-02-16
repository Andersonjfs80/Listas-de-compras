namespace app_backend_produto.domain.Models;

public class TipoPrecoModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    // Navegação
    public ICollection<ProdutoPrecoModel> Precos { get; set; } = new List<ProdutoPrecoModel>();
}

