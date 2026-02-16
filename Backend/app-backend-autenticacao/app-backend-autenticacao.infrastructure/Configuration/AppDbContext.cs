using Microsoft.EntityFrameworkCore;
using app_backend_autenticacao.domain.Models;

namespace app_backend_autenticacao.infrastructure.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UsuarioModel> Usuarios { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioModel>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Documento).HasMaxLength(20);
            entity.Property(e => e.Apelido).HasMaxLength(100);
            entity.Property(e => e.SenhaHash).IsRequired();
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
            
            // Controle de Sessão
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.RefreshTokenExpiryTime);
            entity.Property(e => e.DataAtualizacaoSenha);
        });
    }
}

