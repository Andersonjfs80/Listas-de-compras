using app_backend_autenticacao.domain.Models;
using Microsoft.EntityFrameworkCore;
using Core_Logs.Interfaces;

namespace app_backend_autenticacao.infrastructure.Configuration;

public static class DbInitializer
{
    public static void Seed(AppDbContext context, ISecurityService securityService)
    {
        var dataAtual = DateTime.UtcNow;

        // --- Usuário Admin ---
        var adminEmail = "andersonyx@hotmail.com";
        var admin = context.Usuarios.FirstOrDefault(u => u.Email == adminEmail);
        
        if (admin == null)
        {
            admin = new UsuarioModel 
            { 
                Id = Guid.NewGuid(), 
                Nome = "Anderson Usuario Real", 
                Email = adminEmail,
                SenhaHash = securityService.HashPassword("Dev@123456"),
                Ativo = true, 
                DataCriacao = dataAtual,
                Documento = "00000000000",
                Apelido = "Anderson"
            };
            context.Usuarios.Add(admin);
            context.SaveChanges();
        }
    }
}
