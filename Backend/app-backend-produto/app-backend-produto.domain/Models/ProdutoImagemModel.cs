using app_backend_produto.domain.Types;

namespace app_backend_produto.domain.Models;

public class ProdutoImagemModel
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Conteudo { get; set; } = string.Empty; // Pode ser URL ou Base64
    public TipoImagem Tipo { get; set; }
    public bool Favorito { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataInativacao { get; set; }

    // Navegação
    public ProdutoModel Produto { get; set; } = null!;
}
