using Microsoft.EntityFrameworkCore;
using app_backend_lista_compras.domain.Models;

namespace app_backend_lista_compras.infrastructure.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ListaComprasModel> ListasCompras { get; set; }
    public DbSet<ItemListaModel> ItensLista { get; set; }
    public DbSet<OfertaModel> Ofertas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ListaComprasModel
        modelBuilder.Entity<ListaComprasModel>(entity =>
        {
            entity.ToTable("ListasCompras");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UsuarioId).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);

            entity.HasMany(e => e.Itens)
                  .WithOne(i => i.Lista)
                  .HasForeignKey(i => i.ListaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ItemListaModel
        modelBuilder.Entity<ItemListaModel>(entity =>
        {
            entity.ToTable("ItensLista");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NomeProduto).IsRequired().HasMaxLength(300);
            entity.Property(e => e.NomeCurto).HasMaxLength(150);
            entity.Property(e => e.Quantidade).IsRequired().HasColumnType("decimal(10,3)");
            entity.Property(e => e.UnidadeMedida).IsRequired().HasMaxLength(20);
            
            entity.Property(e => e.QuantidadeConversao).HasColumnType("decimal(10,3)");
            entity.Property(e => e.UnidadeMedidaConversao).HasMaxLength(20);
            
            entity.Property(e => e.PrecoCompra).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PrecoVenda).HasColumnType("decimal(10,2)");

            entity.Property(e => e.CategoriaNome).HasMaxLength(200);
            entity.Property(e => e.Imagem).HasMaxLength(500);
            entity.Property(e => e.DataCadastro).IsRequired();
        });

        // OfertaModel
        modelBuilder.Entity<OfertaModel>(entity =>
        {
            entity.ToTable("Ofertas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NomeProduto).IsRequired().HasMaxLength(300);
            entity.Property(e => e.PrecoAtual).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.PrecoAnterior).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Imagem).HasMaxLength(500);
            entity.Property(e => e.CategoriaNome).HasMaxLength(200);
            entity.Property(e => e.DataInicio).IsRequired();
            entity.Property(e => e.DataFim).IsRequired();
            entity.Property(e => e.DataCadastro).IsRequired();
            entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
        });
    }
}
