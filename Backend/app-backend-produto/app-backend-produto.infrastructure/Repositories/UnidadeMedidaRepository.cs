using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class UnidadeMedidaRepository : IUnidadeMedidaRepository
{
    private readonly AppDbContext _context;

    public UnidadeMedidaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UnidadeMedidaModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UnidadesMedida.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<UnidadeMedidaModel>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UnidadesMedida.ToListAsync(cancellationToken);
    }

    public async Task<UnidadeMedidaModel?> ObterPorSiglaAsync(string sigla, CancellationToken cancellationToken = default)
    {
        return await _context.UnidadesMedida
            .FirstOrDefaultAsync(u => u.Sigla == sigla, cancellationToken);
    }

    public async Task<UnidadeMedidaModel> AdicionarAsync(UnidadeMedidaModel unidadeMedida, CancellationToken cancellationToken = default)
    {
        await _context.UnidadesMedida.AddAsync(unidadeMedida, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return unidadeMedida;
    }

    public async Task AtualizarAsync(UnidadeMedidaModel unidadeMedida, CancellationToken cancellationToken = default)
    {
        _context.UnidadesMedida.Update(unidadeMedida);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unidadeMedida = await _context.UnidadesMedida.FindAsync(new object[] { id }, cancellationToken);
        if (unidadeMedida != null)
        {
            _context.UnidadesMedida.Remove(unidadeMedida);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

