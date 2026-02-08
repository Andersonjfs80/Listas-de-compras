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

        // Gerar chave de cache com prefixo de sessão
        var cacheKey = $"produtos:listagem:{request.Headers.SessionId}:{request.PageNumber}:{request.PageSize}:" +
                       $"{request.Nome}:{request.CategoriaId}:{request.Ativo}:{request.OrdenarPor}:{request.OrdemCrescente}";

        // Tentar recuperar do cache
        var cachedResult = await _cacheService.GetAsync<ProdutoListagemQueryResponse>(
            cacheKey, cancellationToken);

        if (cachedResult != null)
            return cachedResult;

        // Se não houver cache, consultar repositório
        var (items, totalCount) = await _repository.ObterComPaginacaoAsync(
            request.PageNumber,
            request.PageSize,
            request.Nome,
            request.CategoriaId,
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
            CategoriaPrincipal = p.Categoria?.Nome ?? string.Empty,
            TabelaPrecoPrincipal = p.Precos.FirstOrDefault(pr => pr.Principal && pr.Ativo)?.TipoPreco?.Nome,
            PrecoPrincipal = p.Precos.FirstOrDefault(pr => pr.Principal && pr.Ativo)?.Valor
        }).ToList();

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

        // Armazenar no cache com TTL de 5 minutos
        await _cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return response;
    }
}

