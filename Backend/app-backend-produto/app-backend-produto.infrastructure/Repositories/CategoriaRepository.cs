using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoriaModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.OwnerId)
            .Include(c => c.SubCategorias)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<CategoriaModel>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CategoriaModel>> ObterSubCategoriasAsync(Guid ownerCategoriaId, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Where(c => c.OwnerId == ownerCategoriaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoriaModel> AdicionarAsync(CategoriaModel categoria, CancellationToken cancellationToken = default)
    {
        await _context.Categorias.AddAsync(categoria, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return categoria;
    }

    public async Task AtualizarAsync(CategoriaModel categoria, CancellationToken cancellationToken = default)
    {
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var categoria = await _context.Categorias.FindAsync(new object[] { id }, cancellationToken);
        if (categoria != null)
        {
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

