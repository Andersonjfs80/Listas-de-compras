using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class ProdutoPrecoRepository : IProdutoPrecoRepository
{
    private readonly AppDbContext _context;

    public ProdutoPrecoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProdutoPrecoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProdutoPrecos
            .Include(p => p.Produto)
            .Include(p => p.TipoPreco)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ProdutoPrecoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.ProdutoPrecos
			.Include(p => p.TipoPreco)
            .Where(p => p.ProdutoId == produtoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProdutoPrecoModel> AdicionarAsync(ProdutoPrecoModel preco, CancellationToken cancellationToken = default)
    {
        await _context.ProdutoPrecos.AddAsync(preco, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return preco;
    }

    public async Task AtualizarAsync(ProdutoPrecoModel preco, CancellationToken cancellationToken = default)
    {
        _context.ProdutoPrecos.Update(preco);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var preco = await _context.ProdutoPrecos.FindAsync(new object[] { id }, cancellationToken);
        if (preco != null)
        {
            _context.ProdutoPrecos.Remove(preco);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

