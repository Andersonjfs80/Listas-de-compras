using app_backend_lista_compras.domain.Commands.ListaCompras.Request;
using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using MediatR;
using Mapster;

namespace app_backend_lista_compras.domain.Handlers.ListaCompras
{
	public class ItemListaToggleHandler : IRequestHandler<ItemListaToggleRequest, ItemListaToggleResponse>
	{
		private readonly IItemListaRepository _repository;

		public ItemListaToggleHandler(IItemListaRepository repository)
		{
			_repository = repository;
		}

		public async Task<ItemListaToggleResponse> Handle(ItemListaToggleRequest request, CancellationToken cancellationToken)
		{
			var item = await _repository.ToggleMarcadoAsync(request.ItemId, cancellationToken);
			var response = new ItemListaToggleResponse { Item = item.Adapt<ItemListaResponse>() };
			response.ComMensagem(item.Marcado ? "Item marcado" : "Item desmarcado");
			return response;
		}
	}
}
