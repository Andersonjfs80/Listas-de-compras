using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class TipoEstabelecimentoRepository : BaseRepository<TipoEstabelecimentoModel>, ITipoEstabelecimentoRepository
{
    public TipoEstabelecimentoRepository(AppDbContext context) : base(context)
    {
    }
}
