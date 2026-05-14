using MediatR;
using app_backend_produto.domain.Commands.UnidadeMedida.Requests;
using app_backend_produto.domain.Commands.UnidadeMedida.Responses;
using app_backend_produto.domain.Interfaces.Repositories;
using Mapster;

namespace app_backend_produto.domain.Handlers.UnidadeMedida;

public class ListarUnidadesMedidaQueryHandler(
    IUnidadeMedidaRepository repository) : IRequestHandler<ListarUnidadesMedidaQueryRequest, ListarUnidadesMedidaQueryResponse>
{
    private readonly IUnidadeMedidaRepository _repository = repository;

    public async Task<ListarUnidadesMedidaQueryResponse> Handle(
        ListarUnidadesMedidaQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = new ListarUnidadesMedidaQueryResponse();

        var unidades = await _repository.ObterTodosAsync(cancellationToken);

        response.Unidades = unidades
            .Where(u => u.Ativo)
            .OrderBy(u => u.Sigla)
            .Adapt<IEnumerable<UnidadeMedidaResponse>>();

        return (ListarUnidadesMedidaQueryResponse)response.ComMensagem("Unidades de medida recuperadas com sucesso");
    }
}
