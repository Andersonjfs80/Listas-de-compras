using MediatR;
using Core_Logs.Interfaces;
using app_backend_produto.domain.Commands.TipoEstabelecimento.Requests;
using app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;
using app_backend_produto.domain.Interfaces.Repositories;
using Mapster;

namespace app_backend_produto.domain.Handlers.TipoEstabelecimento;

public class ListarTipoEstabelecimentoQueryHandler(
    ITipoEstabelecimentoRepository repository,
    ICacheService cacheService) : IRequestHandler<ListarTipoEstabelecimentoQueryRequest, ListarTipoEstabelecimentoQueryResponse>
{
    private readonly ITipoEstabelecimentoRepository _repository = repository;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<ListarTipoEstabelecimentoQueryResponse> Handle(
        ListarTipoEstabelecimentoQueryRequest request, 
        CancellationToken cancellationToken)
    {
        var response = new ListarTipoEstabelecimentoQueryResponse();

        // Chave de cache
        var cacheKey = $"TipoEstabelecimento/Listar/{request.Headers.SessionId}";

        // Tentar recuperar do cache
        var cachedResult = await _cacheService.GetAsync<IEnumerable<TipoEstabelecimentoResponse>>(
            cacheKey, cancellationToken);

        if (cachedResult != null)
        {
            response.Tipos = cachedResult;
            return (ListarTipoEstabelecimentoQueryResponse)response.ComMensagem("Tipos de estabelecimento recuperados (cache)");
        }

        // Consultar repositório
        var tipos = await _repository.ObterTodosAsync(cancellationToken);
        
        // Mapear resultado
        response.Tipos = tipos.Adapt<IEnumerable<TipoEstabelecimentoResponse>>();

        // Cachear resultado (1 hora)
        await _cacheService.SetAsync(
            cacheKey,
            response.Tipos,
            TimeSpan.FromHours(1),
            cancellationToken);

        return (ListarTipoEstabelecimentoQueryResponse)response.ComMensagem("Tipos de estabelecimento recuperados com sucesso");
    }
}
