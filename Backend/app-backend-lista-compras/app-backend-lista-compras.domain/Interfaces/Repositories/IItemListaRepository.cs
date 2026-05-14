using app_backend_lista_compras.domain.Models;

namespace app_backend_lista_compras.domain.Interfaces.Repositories;

public interface IItemListaRepository
{
    Task<IEnumerable<ItemListaModel>> ObterPorListaAsync(Guid listaId, CancellationToken cancellationToken = default);
    Task<ItemListaModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ItemListaModel> AdicionarAsync(ItemListaModel item, CancellationToken cancellationToken = default);
    Task<ItemListaModel> ToggleMarcadoAsync(Guid id, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
