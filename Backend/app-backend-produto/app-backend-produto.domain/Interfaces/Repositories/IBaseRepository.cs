
namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<T> AdicionarAsync(T entity, CancellationToken cancellationToken = default);
    Task AtualizarAsync(T entity, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
