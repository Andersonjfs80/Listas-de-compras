using MediatR;
using app_backend_produto.domain.Commands.Produto.Responses;
using Core_Logs.Models.Request;
using System.Text.Json.Serialization;

namespace app_backend_produto.domain.Commands.Produto.Requests;

/// <summary>
/// Request para listagem paginada de produtos
/// </summary>
public class ProdutoListagemQueryRequest : IRequest<ProdutoListagemQueryResponse>
{
    /// <summary>
    /// Headers da requisição (obtidos do BaseController)
    /// </summary>
    [JsonIgnore]
    public RequestHeaders Headers { get; set; } = new();

    /// <summary>
    /// Número da página (começa em 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Tamanho da página (máximo 100)
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Filtro por nome do produto (busca parcial)
    /// </summary>
    public string? Nome { get; set; }

    /// <summary>
    /// Filtro por ID da categoria
    /// </summary>
    public Guid? CategoriaId { get; set; }

    /// <summary>
    /// Filtro por ID do Fornecedor (via tabela de códigos)
    /// </summary>
    public Guid? FornecedorId { get; set; }

    /// <summary>
    /// Filtro por ID do Tipo de Estabelecimento (Categoria de Fornecedor)
    /// </summary>
    public Guid? TipoEstabelecimentoId { get; set; }

    /// <summary>
    /// Filtro por status ativo/inativo
    /// </summary>
    public bool? Ativo { get; set; }

    /// <summary>
    /// Campo para ordenação (Nome, NomeCurto, DataCadastro)
    /// </summary>
    public string OrdenarPor { get; set; } = "Nome";

    /// <summary>
    /// Ordem crescente (true) ou decrescente (false)
    /// </summary>
    public bool OrdemCrescente { get; set; } = true;
}

