using MediatR;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using app_backend_lista_compras.domain.Commands.ListaCompras.Request;
using Mapster;

namespace app_backend_lista_compras.domain.Handlers.ListaCompras;

public class ListaComprasQueryHandler : IRequestHandler<ListaComprasQueryRequest, ListaComprasQueryResponse>
{
    private readonly IListaComprasRepository _repository;

    public ListaComprasQueryHandler(IListaComprasRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListaComprasQueryResponse> Handle(
        ListaComprasQueryRequest request,
        CancellationToken cancellationToken)
    {
        var listas = await _repository.ObterPorUsuarioAsync(request.Headers.UsuarioId.GetValueOrDefault(), cancellationToken);

        var response = new ListaComprasQueryResponse
        {
            Listas = listas.Adapt<List<ListaComprasResponse>>()
        };

        response.ComMensagem("Listas de compras recuperadas com sucesso");
        return response;
    }
}
