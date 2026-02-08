using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class CodigoProdutoRepository : ICodigoProdutoRepository
{
    private readonly AppDbContext _context;

    public CodigoProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CodigoProdutoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CodigosProduto
            .Include(c => c.Produto)
            .Include(c => c.UnidadeMedida)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<CodigoProdutoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.CodigosProduto
            .Include(c => c.UnidadeMedida)
            .Where(c => c.ProdutoId == produtoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CodigoProdutoModel?> ObterPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default)
    {
        return await _context.CodigosProduto
            .Include(c => c.Produto)
            .Include(c => c.UnidadeMedida)
            .FirstOrDefaultAsync(c => c.CodigoBarras == codigoBarras, cancellationToken);
    }

    public async Task<CodigoProdutoModel> AdicionarAsync(CodigoProdutoModel codigoProduto, CancellationToken cancellationToken = default)
    {
        await _context.CodigosProduto.AddAsync(codigoProduto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return codigoProduto;
    }

    public async Task AtualizarAsync(CodigoProdutoModel codigoProduto, CancellationToken cancellationToken = default)
    {
        _context.CodigosProduto.Update(codigoProduto);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var codigoProduto = await _context.CodigosProduto.FindAsync(new object[] { id }, cancellationToken);
        if (codigoProduto != null)
        {
            _context.CodigosProduto.Remove(codigoProduto);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

