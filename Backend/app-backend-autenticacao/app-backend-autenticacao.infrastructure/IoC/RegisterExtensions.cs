using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using app_backend_autenticacao.infrastructure.Configuration;
using app_backend_autenticacao.infrastructure.Repositories;
using app_backend_autenticacao.infrastructure.Mappings;

namespace app_backend_autenticacao.infrastructure.IoC;

public static class RegisterExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(app_backend_autenticacao.domain.Commands.Auth.Requests.LoginRequest).Assembly));
        services.AddMapster();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Banco de Dados
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. Repositórios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }
}

