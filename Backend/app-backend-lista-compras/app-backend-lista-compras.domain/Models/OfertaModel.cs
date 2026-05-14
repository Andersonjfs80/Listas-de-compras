namespace app_backend_lista_compras.domain.Models;

public class OfertaModel
{
    public Guid Id { get; set; }
    public Guid? ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoAtual { get; set; }
    public decimal PrecoAnterior { get; set; }
    public string? Imagem { get; set; }
    public Guid? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool Ativo { get; set; } = true;

    public Guid UsuarioId { get; set; }
    public DateTime DataCadastro { get; set; }
}
