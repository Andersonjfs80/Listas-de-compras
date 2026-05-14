using Microsoft.EntityFrameworkCore;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Models;
using app_backend_lista_compras.infrastructure.Configuration;

namespace app_backend_lista_compras.infrastructure.Repositories;

public class ItemListaRepository : IItemListaRepository
{
    private readonly AppDbContext _context;

    public ItemListaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ItemListaModel>> ObterPorListaAsync(
        Guid listaId, CancellationToken cancellationToken = default)
    {
        return await _context.ItensLista
            .Where(i => i.ListaId == listaId)
            .OrderBy(i => i.CategoriaNome)
            .ThenBy(i => i.NomeProduto)
            .ToListAsync(cancellationToken);
    }

    public async Task<ItemListaModel?> ObterPorIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ItensLista
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<ItemListaModel> AdicionarAsync(
        ItemListaModel item, CancellationToken cancellationToken = default)
    {
        _context.ItensLista.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<ItemListaModel> ToggleMarcadoAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.ItensLista.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Item {id} não encontrado.");

        item.Marcado = !item.Marcado;
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.ItensLista.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            _context.ItensLista.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
