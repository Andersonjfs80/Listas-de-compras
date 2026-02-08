using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace app_backend_produto.infrastructure.IoC;

public static class RegisterExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registro do MediatR
        var domainAssembly = System.Reflection.Assembly.Load("app-backend-produto.domain");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(domainAssembly));
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registro do DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<app_backend_produto.infrastructure.Configuration.AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        // Registro de Repositórios
        services.AddScoped<app_backend_produto.domain.Interfaces.Repositories.IProdutoRepository, 
            app_backend_produto.infrastructure.Repositories.ProdutoRepository>();
        services.AddScoped<app_backend_produto.domain.Interfaces.Repositories.ICategoriaRepository, 
            app_backend_produto.infrastructure.Repositories.CategoriaRepository>();
        services.AddScoped<app_backend_produto.domain.Interfaces.Repositories.IPrecoRepository, 
            app_backend_produto.infrastructure.Repositories.PrecoRepository>();
        services.AddScoped<app_backend_produto.domain.Interfaces.Repositories.ITipoPrecoRepository, 
            app_backend_produto.infrastructure.Repositories.TipoPrecoRepository>();
        services.AddScoped<app_backend_produto.domain.Interfaces.Repositories.ICodigoProdutoRepository, 
            app_backend_produto.infrastructure.Repositories.CodigoProdutoRepository>();
        services.AddScoped<app_backend_produto.domain.Interfaces.Repositories.IUnidadeMedidaRepository, 
            app_backend_produto.infrastructure.Repositories.UnidadeMedidaRepository>();
        
        return services;
    }
}


