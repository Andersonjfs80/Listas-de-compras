using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IPrecoRepository
{
    Task<PrecoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrecoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<PrecoModel> AdicionarAsync(PrecoModel preco, CancellationToken cancellationToken = default);
    Task AtualizarAsync(PrecoModel preco, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

