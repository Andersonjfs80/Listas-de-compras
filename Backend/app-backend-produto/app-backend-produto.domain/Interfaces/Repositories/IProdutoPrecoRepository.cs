using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IProdutoPrecoRepository
{
    Task<ProdutoPrecoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoPrecoModel>> ObterPorProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<ProdutoPrecoModel> AdicionarAsync(ProdutoPrecoModel preco, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ProdutoPrecoModel preco, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
