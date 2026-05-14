using app_backend_lista_compras.domain.Commands.ListaCompras.Request;
using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using MediatR;

namespace app_backend_lista_compras.domain.Handlers.ListaCompras
{
	public class ItemListaRemoverHandler : IRequestHandler<ItemListaRemoverRequest, ItemListaRemoverResponse>
	{
		private readonly IItemListaRepository _repository;
		private readonly IListaComprasRepository _listaRepository;

		public ItemListaRemoverHandler(IItemListaRepository repository, IListaComprasRepository listaRepository)
		{
			_repository = repository;
			_listaRepository = listaRepository;
		}

		public async Task<ItemListaRemoverResponse> Handle(ItemListaRemoverRequest request, CancellationToken cancellationToken)
		{
			await _repository.RemoverAsync(request.ItemId, cancellationToken);
			var response = new ItemListaRemoverResponse();
			response.ComMensagem("Item removido com sucesso");
			return response;
		}
	}
}
