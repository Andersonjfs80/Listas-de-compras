using app_backend_lista_compras.domain.Models;

namespace app_backend_lista_compras.domain.Interfaces.Repositories;

public interface IListaComprasRepository
{
    Task<IEnumerable<ListaComprasModel>> ObterPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<ListaComprasModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListaComprasModel> CriarAsync(ListaComprasModel lista, CancellationToken cancellationToken = default);
    Task<ListaComprasModel> AtualizarAsync(ListaComprasModel lista, CancellationToken cancellationToken = default);
    Task DeletarAsync(Guid id, CancellationToken cancellationToken = default);
}
