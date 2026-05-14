using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.infrastructure.Configuration;
using app_backend_lista_compras.infrastructure.Repositories;

namespace app_backend_lista_compras.infrastructure.IoC;

public static class RegisterExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var domainAssembly = System.Reflection.Assembly.Load("app-backend-lista-compras.domain");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(domainAssembly));
        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IListaComprasRepository, ListaComprasRepository>();
        services.AddScoped<IItemListaRepository, ItemListaRepository>();
        services.AddScoped<IOfertaRepository, OfertaRepository>();

        return services;
    }
}
