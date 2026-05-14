using Microsoft.EntityFrameworkCore;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Models;
using app_backend_lista_compras.infrastructure.Configuration;

namespace app_backend_lista_compras.infrastructure.Repositories;

public class ListaComprasRepository : IListaComprasRepository
{
    private readonly AppDbContext _context;

    public ListaComprasRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ListaComprasModel>> ObterPorUsuarioAsync(
        Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.ListasCompras
            .Include(l => l.Itens)
            .Where(l => l.UsuarioId == usuarioId && l.Ativo)
            .OrderByDescending(l => l.DataCadastro)
            .ToListAsync(cancellationToken);
    }

    public async Task<ListaComprasModel?> ObterPorIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ListasCompras
            .Include(l => l.Itens)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<ListaComprasModel> CriarAsync(
        ListaComprasModel lista, CancellationToken cancellationToken = default)
    {
        _context.ListasCompras.Add(lista);
        await _context.SaveChangesAsync(cancellationToken);
        return lista;
    }

    public async Task<ListaComprasModel> AtualizarAsync(
        ListaComprasModel lista, CancellationToken cancellationToken = default)
    {
        _context.ListasCompras.Update(lista);
        await _context.SaveChangesAsync(cancellationToken);
        return lista;
    }

    public async Task DeletarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lista = await _context.ListasCompras.FindAsync(new object[] { id }, cancellationToken);
        if (lista != null)
        {
            lista.Ativo = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
