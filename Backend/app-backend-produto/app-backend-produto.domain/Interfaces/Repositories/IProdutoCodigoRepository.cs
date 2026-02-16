using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IProdutoCodigoRepository
{
    Task<ProdutoCodigoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoCodigoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<ProdutoCodigoModel?> ObterPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default);
    Task<ProdutoCodigoModel> AdicionarAsync(ProdutoCodigoModel codigoProduto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ProdutoCodigoModel codigoProduto, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
