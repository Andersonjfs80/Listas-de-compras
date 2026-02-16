using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IProdutoRepository
{
    Task<ProdutoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoModel>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<ProdutoModel> AdicionarAsync(ProdutoModel produto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ProdutoModel produto, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtém produtos com paginação e filtros
    /// </summary>
    Task<(IEnumerable<ProdutoModel> Items, int TotalCount)> ObterComPaginacaoAsync(
        int pageNumber,
        int pageSize,
        string? nome,
        Guid? categoriaId,
        Guid? fornecedorId,
        Guid? tipoEstabelecimentoId,
        bool? ativo,
        string ordenarPor,
        bool ordemCrescente,
        CancellationToken cancellationToken = default);
}

