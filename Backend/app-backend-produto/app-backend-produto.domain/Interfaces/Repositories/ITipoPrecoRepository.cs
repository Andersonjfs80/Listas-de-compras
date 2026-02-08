using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface ITipoPrecoRepository
{
    Task<TipoPrecoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TipoPrecoModel>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<TipoPrecoModel> AdicionarAsync(TipoPrecoModel tipoPreco, CancellationToken cancellationToken = default);
    Task AtualizarAsync(TipoPrecoModel tipoPreco, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

