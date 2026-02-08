using System.Text.Json.Serialization;
using Core_Logs.Commands;

namespace app_backend_produto.domain.Commands.Produto.Responses;

/// <summary>
/// DTO para item de produto na listagem
/// </summary>
public class ProdutoListagemDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeCurto { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public string CategoriaPrincipal { get; set; } = string.Empty;
    public string? TabelaPrecoPrincipal { get; set; }
    public decimal? PrecoPrincipal { get; set; }
}

/// <summary>
/// Response para listagem paginada de produtos (herda de BaseCommand)
/// </summary>
public class ProdutoListagemQueryResponse : BaseCommand
{
    public List<ProdutoListagemDto> Produtos { get; set; } = new();
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

