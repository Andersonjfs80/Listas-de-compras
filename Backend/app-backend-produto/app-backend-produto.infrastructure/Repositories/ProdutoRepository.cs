using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Configuration;

namespace app_backend_produto.infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProdutoModel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Precos)
            .Include(p => p.Codigos)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ProdutoModel>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProdutoModel> AdicionarAsync(ProdutoModel produto, CancellationToken cancellationToken = default)
    {
        await _context.Produtos.AddAsync(produto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return produto;
    }

    public async Task AtualizarAsync(ProdutoModel produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var produto = await _context.Produtos.FindAsync(new object[] { id }, cancellationToken);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<(IEnumerable<ProdutoModel> Items, int TotalCount)> ObterComPaginacaoAsync(
        int pageNumber,
        int pageSize,
        string? nome,
        Guid? categoriaId,
        bool? ativo,
        string ordenarPor,
        bool ordemCrescente,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Precos.Where(pr => pr.Principal && pr.Ativo))
                .ThenInclude(pr => pr.TipoPreco)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(p => p.Nome.Contains(nome) || p.NomeCurto.Contains(nome));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == ativo.Value);
        }

        // Contar total antes da paginação
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar ordenação
        query = ordenarPor.ToLower() switch
        {
            "nomecurto" => ordemCrescente 
                ? query.OrderBy(p => p.NomeCurto) 
                : query.OrderByDescending(p => p.NomeCurto),
            "datacadastro" => ordemCrescente 
                ? query.OrderBy(p => p.DataCadastro) 
                : query.OrderByDescending(p => p.DataCadastro),
            _ => ordemCrescente 
                ? query.OrderBy(p => p.Nome) 
                : query.OrderByDescending(p => p.Nome)
        };

        // Aplicar paginação
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

