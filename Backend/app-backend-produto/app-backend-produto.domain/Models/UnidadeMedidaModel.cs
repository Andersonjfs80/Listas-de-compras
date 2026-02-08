namespace app_backend_produto.domain.Models;

public class UnidadeMedidaModel
{
    public Guid Id { get; set; }
    public string Sigla { get; set; } = string.Empty; // UN, CX, KG, etc
    public string Descricao { get; set; } = string.Empty;
    public decimal FatorConversao { get; set; } = 1; // Quantidade de conversão
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    // Navegação
    public ICollection<CodigoProdutoModel> CodigosProduto { get; set; } = new List<CodigoProdutoModel>();
}

