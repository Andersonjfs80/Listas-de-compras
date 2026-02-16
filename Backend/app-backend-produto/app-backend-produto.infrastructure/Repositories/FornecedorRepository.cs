using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class FornecedorRepository : BaseRepository<FornecedorModel>, IFornecedorRepository
{
    private readonly AppDbContext _context;

    public FornecedorRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FornecedorModel>> ObterPorTipoAsync(Guid tipoId, CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .Include(f => f.TipoEstabelecimento)
            .Where(f => f.TipoEstabelecimentoId == tipoId)
            .ToListAsync(cancellationToken);
    }
}
