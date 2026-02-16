using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace app_backend_produto.infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AdicionarAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task AtualizarAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ObterPorIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
