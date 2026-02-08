using Microsoft.EntityFrameworkCore;
using app_backend_produto.domain.Models;

namespace app_backend_produto.infrastructure.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ProdutoModel> Produtos => Set<ProdutoModel>();
    public DbSet<CategoriaModel> Categorias => Set<CategoriaModel>();
    public DbSet<PrecoModel> Precos => Set<PrecoModel>();
    public DbSet<TipoPrecoModel> TiposPreco => Set<TipoPrecoModel>();
    public DbSet<CodigoProdutoModel> CodigosProduto => Set<CodigoProdutoModel>();
    public DbSet<UnidadeMedidaModel> UnidadesMedida => Set<UnidadeMedidaModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Produto
        modelBuilder.Entity<ProdutoModel>(entity =>
        {
            entity.ToTable("Produtos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeCurto).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Índices para otimização de consultas
            entity.HasIndex(e => e.Nome);
            entity.HasIndex(e => e.NomeCurto);
            entity.HasIndex(e => e.Ativo);
            entity.HasIndex(e => e.DataCadastro);
            entity.HasIndex(e => e.CategoriaId);
            entity.HasIndex(e => new { e.Ativo, e.DataCadastro }); // Índice composto
        });

        // Configuração de Categoria
        modelBuilder.Entity<CategoriaModel>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            // Auto-relacionamento
            entity.HasOne(e => e.OwnerCategoria)
                .WithMany(c => c.SubCategorias)
                .HasForeignKey(e => e.OwnerCategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração de Preco
        modelBuilder.Entity<PrecoModel>(entity =>
        {
            entity.ToTable("Precos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Principal).IsRequired();
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.Precos)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.TipoPreco)
                .WithMany(t => t.Precos)
                .HasForeignKey(e => e.TipoPrecoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração de TipoPreco
        modelBuilder.Entity<TipoPrecoModel>(entity =>
        {
            entity.ToTable("TiposPreco");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
        });

        // Configuração de CodigoProduto
        modelBuilder.Entity<CodigoProdutoModel>(entity =>
        {
            entity.ToTable("CodigosProduto");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoProduto).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CodigoBarras).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.Codigos)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.UnidadeMedida)
                .WithMany(u => u.CodigosProduto)
                .HasForeignKey(e => e.UnidadeMedidaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração de UnidadeMedida
        modelBuilder.Entity<UnidadeMedidaModel>(entity =>
        {
            entity.ToTable("UnidadesMedida");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Sigla).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FatorConversao).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
        });
    }
}

