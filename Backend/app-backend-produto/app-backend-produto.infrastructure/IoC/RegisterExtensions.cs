using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Mapster;
using System.Reflection;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.infrastructure.Repositories;
using app_backend_produto.infrastructure.Configuration;

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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        // Registro de Repositórios
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProdutoPrecoRepository, ProdutoPrecoRepository>();
        services.AddScoped<ITipoPrecoRepository, TipoPrecoRepository>();
        services.AddScoped<IProdutoCodigoRepository, ProdutoCodigoRepository>();
        services.AddScoped<IUnidadeMedidaRepository, UnidadeMedidaRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<ITipoEstabelecimentoRepository, TipoEstabelecimentoRepository>();

        
        // Scan for Mapster Configurations
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }
}


