using app_backend_lista_compras.domain.Models;

namespace app_backend_lista_compras.domain.Interfaces.Repositories;

public interface IOfertaRepository
{
    Task<(IEnumerable<OfertaModel> Items, int TotalCount)> ObterAtivasAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
