using MediatR;
using Core_Logs.Interfaces;
using app_backend_produto.domain.Commands.Produto.Requests;
using app_backend_produto.domain.Commands.Produto.Responses;
using app_backend_produto.domain.Interfaces.Repositories;

namespace app_backend_produto.domain.Handlers.Produto;

/// <summary>
/// Handler para listagem paginada de produtos com cache Redis
/// </summary>
public class ProdutoListagemQueryHandler : IRequestHandler<ProdutoListagemQueryRequest, ProdutoListagemQueryResponse>
{
    private readonly IProdutoRepository _repository;
    private readonly ICacheService _cacheService;

    public ProdutoListagemQueryHandler(
        IProdutoRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<ProdutoListagemQueryResponse> Handle(
        ProdutoListagemQueryRequest request,
        CancellationToken cancellationToken)
    {
        // Validar parâmetros
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 20;
        if (request.PageSize > 100) request.PageSize = 100;

        // Se não houver cache, consultar repositório
        var (items, totalCount) = await _repository.ObterComPaginacaoAsync(
            request.PageNumber,
            request.PageSize,
            request.Nome,
            request.CategoriaId,
            request.FornecedorId,
            request.TipoEstabelecimentoId,
            request.Ativo,
            request.OrdenarPor,
            request.OrdemCrescente,
            cancellationToken);

        // Mapear para DTO
        var produtosDto = items.Select(p => new ProdutoListagemDto
        {
            Id = p.Id,
            Nome = p.Nome,
            NomeCurto = p.NomeCurto,
            Ativo = p.Ativo,
            // Recuperar Categoria Principal da lista N-N
            CategoriaPrincipal = p.ProdutoCategorias.FirstOrDefault(pc => pc.Tipo == app_backend_produto.domain.Enums.TipoCategoria.Principal && pc.Ativo)?.Categoria?.Nome ?? string.Empty,
            TabelaPrecoPrincipal = p.ProdutoPrecos.FirstOrDefault(pr => pr.Principal && pr.Ativo)?.TipoPreco?.Nome,
            PrecoPrincipal = p.ProdutoPrecos.FirstOrDefault(pr => pr.Principal && pr.Ativo)?.Valor ?? 0
        }).ToList(); // Adicionei default 0 para Valor se for null

        var response = new ProdutoListagemQueryResponse
        {
            Produtos = produtosDto,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            CurrentPage = request.PageNumber,
            PageSize = request.PageSize
        };

        // Definir sucesso
        response.ComMensagem("Listagem de produtos recuperada com sucesso");

        return response;
    }
}

