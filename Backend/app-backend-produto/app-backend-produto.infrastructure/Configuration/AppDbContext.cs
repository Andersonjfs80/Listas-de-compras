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
    public DbSet<ProdutoCategoriaModel> ProdutoCategorias => Set<ProdutoCategoriaModel>();
    public DbSet<ProdutoPrecoModel> ProdutoPrecos => Set<ProdutoPrecoModel>();
    public DbSet<TipoPrecoModel> TiposPreco => Set<TipoPrecoModel>();
    public DbSet<ProdutoCodigoModel> CodigosProduto => Set<ProdutoCodigoModel>();
    public DbSet<UnidadeMedidaModel> UnidadesMedida => Set<UnidadeMedidaModel>();
    public DbSet<ProdutoImagemModel> ProdutoImagens => Set<ProdutoImagemModel>();
    public DbSet<TipoEstabelecimentoModel> TiposEstabelecimento => Set<TipoEstabelecimentoModel>();
    public DbSet<FornecedorModel> Fornecedores => Set<FornecedorModel>();

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
            
            // Relacionamento N-N é configurado na entidade de junção ProdutoCategoriaModel
            
            // Índices para otimização de consultas
            entity.HasIndex(e => e.Nome);
            entity.HasIndex(e => e.NomeCurto);
            entity.HasIndex(e => e.Ativo);
            entity.HasIndex(e => e.DataCadastro);
            entity.HasIndex(e => new { e.Ativo, e.DataCadastro }); // Índice composto
        });

        // Configuração de ProdutoCategoria (Tabela de Junção N-N)
        modelBuilder.Entity<ProdutoCategoriaModel>(entity =>
        {
            entity.ToTable("ProdutoCategorias");
            entity.HasKey(e => new { e.ProdutoId, e.CategoriaId }); // Chave composta

            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ProdutoCategorias)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.ProdutoCategorias)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Tipo).IsRequired();
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
            // Auto-relacionamento
            entity.HasOne(e => e.Owner)
                .WithMany(c => c.SubCategorias)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração de ProdutoPreco
        modelBuilder.Entity<ProdutoPrecoModel>(entity =>
        {
            entity.ToTable("ProdutoPrecos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Principal).IsRequired();
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ProdutoPrecos)
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

        // Configuração de ProdutoCodigoModel
        modelBuilder.Entity<ProdutoCodigoModel>(entity =>
        {
            entity.ToTable("CodigosProduto");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoProduto).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CodigoBarras).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ProdutoCodigos)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.UnidadeMedida)
                .WithMany(u => u.CodigosProduto)
                .HasForeignKey(e => e.UnidadeMedidaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Fornecedor)
                .WithMany(f => f.Codigos)
                .HasForeignKey(e => e.FornecedorId)
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

        // Configuração de ProdutoImagem
        modelBuilder.Entity<ProdutoImagemModel>(entity =>
        {
            entity.ToTable("ProdutoImagens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Conteudo).IsRequired(); // Removido MaxLength para suportar Base64 (nvarchar(max))
            entity.Property(e => e.Tipo).IsRequired();
            entity.Property(e => e.Favorito).IsRequired();
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.UsuarioId).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ProdutoImagens)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ProdutoId);
            entity.HasIndex(e => e.Ativo);
        });

        // Configuração de TipoEstabelecimento
        modelBuilder.Entity<TipoEstabelecimentoModel>(entity =>
        {
            entity.ToTable("TiposEstabelecimento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
        });

        // Configuração de Fornecedor
        modelBuilder.Entity<FornecedorModel>(entity =>
        {
            entity.ToTable("Fornecedores");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeFantasia).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cnpj).HasMaxLength(20);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();

            entity.HasOne(e => e.TipoEstabelecimento)
                .WithMany(t => t.Fornecedores)
                .HasForeignKey(e => e.TipoEstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }


}

