using MediatR;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Models;
using app_backend_lista_compras.domain.Commands.ListaCompras.Request;
using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using Mapster;

namespace app_backend_lista_compras.domain.Handlers.ListaCompras;

public class ListaComprasCriarHandler : IRequestHandler<ListaComprasCriarRequest, ListaComprasCriarResponse>
{
    private readonly IListaComprasRepository _repository;

    public ListaComprasCriarHandler(IListaComprasRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListaComprasCriarResponse> Handle(ListaComprasCriarRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            request.Nome = $"Lista {DateTime.Now:dd/MM}";

        var lista = new ListaComprasModel
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            UsuarioId = request.Headers.UsuarioId.GetValueOrDefault(),
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        var criada = await _repository.CriarAsync(lista, cancellationToken);

        var response = new ListaComprasCriarResponse
        {
            Lista = criada.Adapt<ListaComprasResponse>()
        };

        response.ComMensagem("Lista de compras criada com sucesso");
        return response;
    }
}
