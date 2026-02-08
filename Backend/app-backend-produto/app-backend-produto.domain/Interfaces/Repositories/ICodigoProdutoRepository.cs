using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface ICodigoProdutoRepository
{
    Task<CodigoProdutoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CodigoProdutoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<CodigoProdutoModel?> ObterPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default);
    Task<CodigoProdutoModel> AdicionarAsync(CodigoProdutoModel codigoProduto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(CodigoProdutoModel codigoProduto, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

