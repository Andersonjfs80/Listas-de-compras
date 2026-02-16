using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Interfaces.Repositories;

public interface IFornecedorRepository : IBaseRepository<FornecedorModel>
{
    Task<IEnumerable<FornecedorModel>> ObterPorTipoAsync(Guid tipoId, CancellationToken cancellationToken = default);
}
