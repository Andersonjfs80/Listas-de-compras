using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IUnidadeMedidaRepository
{
    Task<UnidadeMedidaModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UnidadeMedidaModel>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<UnidadeMedidaModel?> ObterPorSiglaAsync(string sigla, CancellationToken cancellationToken = default);
    Task<UnidadeMedidaModel> AdicionarAsync(UnidadeMedidaModel unidadeMedida, CancellationToken cancellationToken = default);
    Task AtualizarAsync(UnidadeMedidaModel unidadeMedida, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

