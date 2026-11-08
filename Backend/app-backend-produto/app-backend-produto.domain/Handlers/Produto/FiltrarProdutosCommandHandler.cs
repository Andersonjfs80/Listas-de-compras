using MediatR;
using Core_Logs.Interfaces;
using app_backend_produto.domain.Commands.Produto.Requests;
using app_backend_produto.domain.Commands.Produto.Responses;
using app_backend_produto.domain.Interfaces.Repositories;
using Mapster;
using Core_Logs.Commands;

namespace app_backend_produto.domain.Handlers.Produto;

/// <summary>
/// Handler para listagem paginada de produtos com cache Redis
/// </summary>
public class FiltrarProdutosCommandHandler : IRequestHandler<FiltrarProdutosCommandRequest, FiltrarProdutosCommandResponse>
{
    private readonly IProdutoRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogCustom _logCustom;

    public FiltrarProdutosCommandHandler(
        IProdutoRepository repository,
        ICacheService cacheService,
        ILogCustom logCustom)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logCustom = logCustom;
    }

    public async Task<FiltrarProdutosCommandResponse> Handle(
        FiltrarProdutosCommandRequest request,
        CancellationToken cancellationToken)
    {
        // Validar parâmetros
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 20;
        if (request.PageSize > 100) request.PageSize = 100;

        // Montar a chave de cache com todos os filtros
        var cacheKey = $"produtos:list:{request.PageNumber}:{request.PageSize}:{request.Nome}:{request.CategoriaId}:{request.FornecedorId}:{request.TipoEstabelecimentoId}:{request.Ativo}:{request.OrdenarPor}:{request.OrdemCrescente}";

        // Tentar buscar do cache
        FiltrarProdutosCommandResponse? cachedResponse = null;
        try
        {
            cachedResponse = await _cacheService.GetAsync<FiltrarProdutosCommandResponse>(cacheKey, cancellationToken);
        }
        catch (Exception ex)
        {
            _logCustom.AdicionarLog($"Falha ao acessar o cache Redis para a chave {cacheKey}: {ex.Message}", "FiltrarProdutosCommandHandler", ex.StackTrace);
            // Segue o jogo, fará a consulta no banco de dados
        }

        if (cachedResponse != null)
        {
            return cachedResponse;
        }

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

        // Mapear para DTO usando Mapster
        var produtosDto = items.Adapt<List<ProdutoResponse>>();

        var response = new FiltrarProdutosCommandResponse
        {
            Produtos = produtosDto,
            Paginacao = new PaginacaoInfoResponse
            {
                TotalItems = totalCount,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize
            }
        };

        // Definir sucesso
        response.ComMensagem("Listagem de produtos recuperada com sucesso");

        // Salvar no Cache por 10 minutos (com fallback caso Redis caia)
        try
        {
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10), cancellationToken);
        }
        catch (Exception ex)
        {
            _logCustom.AdicionarLog($"Falha ao salvar no cache Redis para a chave {cacheKey}: {ex.Message}", "FiltrarProdutosCommandHandler", ex.StackTrace);
        }

        return response;
    }
}

