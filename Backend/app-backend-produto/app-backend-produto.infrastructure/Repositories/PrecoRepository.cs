using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class PrecoRepository : IPrecoRepository
{
    private readonly AppDbContext _context;

    public PrecoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PrecoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Precos
            .Include(p => p.Produto)
            .Include(p => p.TipoPreco)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PrecoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.Precos
            .Include(p => p.TipoPreco)
            .Where(p => p.ProdutoId == produtoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PrecoModel> AdicionarAsync(PrecoModel preco, CancellationToken cancellationToken = default)
    {
        await _context.Precos.AddAsync(preco, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return preco;
    }

    public async Task AtualizarAsync(PrecoModel preco, CancellationToken cancellationToken = default)
    {
        _context.Precos.Update(preco);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var preco = await _context.Precos.FindAsync(new object[] { id }, cancellationToken);
        if (preco != null)
        {
            _context.Precos.Remove(preco);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

