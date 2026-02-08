using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class TipoPrecoRepository : ITipoPrecoRepository
{
    private readonly AppDbContext _context;

    public TipoPrecoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TipoPrecoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TiposPreco.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<TipoPrecoModel>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TiposPreco.ToListAsync(cancellationToken);
    }

    public async Task<TipoPrecoModel> AdicionarAsync(TipoPrecoModel tipoPreco, CancellationToken cancellationToken = default)
    {
        await _context.TiposPreco.AddAsync(tipoPreco, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return tipoPreco;
    }

    public async Task AtualizarAsync(TipoPrecoModel tipoPreco, CancellationToken cancellationToken = default)
    {
        _context.TiposPreco.Update(tipoPreco);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tipoPreco = await _context.TiposPreco.FindAsync(new object[] { id }, cancellationToken);
        if (tipoPreco != null)
        {
            _context.TiposPreco.Remove(tipoPreco);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

