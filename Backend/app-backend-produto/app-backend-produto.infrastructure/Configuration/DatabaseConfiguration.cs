using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace app_backend_produto.infrastructure.Configuration;

public static class DatabaseConfiguration
{
    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var logCustom = scope.ServiceProvider.GetRequiredService<Core_Logs.Interfaces.ILogCustom>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    logCustom.AdicionarLog("app-backend-produto: Tentando aplicar migrações...");
                    context.Database.Migrate();
                    logCustom.AdicionarLog("app-backend-produto: Migrações aplicadas com sucesso!");
                    
                    // Se estiver em desenvolvimento, gera massa de dados
                    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
                    if (!env.IsProduction())
                    {
                        DbInitializer.Seed(context);
                    }
                    
                    logCustom.EnviarLogAsync().GetAwaiter().GetResult();
                    break;
                }
                catch (Exception ex)
                {
                    logCustom.AdicionarErro($"app-backend-produto: Tentativa de migração falhou. Retries restantes: {retries}", ex);
                    logCustom.EnviarLogAsync().GetAwaiter().GetResult();
                    
                    retries--;
                    if (retries == 0) 
                    {
                        throw; // Lança exceção para reiniciar container
                    }
                    System.Threading.Thread.Sleep(5000); // 5 segundos
                }
            }
        }
    }
}
