using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface ICategoriaRepository
{
    Task<CategoriaModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoriaModel>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoriaModel>> ObterSubCategoriasAsync(Guid ownerCategoriaId, CancellationToken cancellationToken = default);
    Task<CategoriaModel> AdicionarAsync(CategoriaModel categoria, CancellationToken cancellationToken = default);
    Task AtualizarAsync(CategoriaModel categoria, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

