using Microsoft.EntityFrameworkCore;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Models;
using app_backend_lista_compras.infrastructure.Configuration;

namespace app_backend_lista_compras.infrastructure.Repositories;

public class OfertaRepository : IOfertaRepository
{
    private readonly AppDbContext _context;

    public OfertaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<OfertaModel> Items, int TotalCount)> ObterAtivasAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Ofertas
            .Where(o => o.Ativo && o.DataFim >= DateTime.UtcNow)
            .OrderBy(o => o.DataFim);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
