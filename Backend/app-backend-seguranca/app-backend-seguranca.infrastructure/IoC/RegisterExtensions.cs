using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using app_backend_seguranca.domain.Interfaces.Services;
using app_backend_seguranca.infrastructure.Services;
using app_backend_seguranca.domain.Models.WhatsApp;
using app_backend_seguranca.infrastructure.Mappings;
using Mapster;

namespace app_backend_seguranca.infrastructure.IoC;

public static class RegisterExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(app_backend_seguranca.domain.Handlers.Messaging.ValidarSmsHandler).Assembly));
        services.AddMapster();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configurações
        services.Configure<WhatsAppSettings>(configuration.GetSection("WhatsAppSettings"));

        // 2. Serviços de Segurança/Mensageria
        services.AddHttpClient<IWhatsAppProvider, WhatsAppCloudApiProvider>();
        services.AddScoped<IMessagingService, MessagingService>();
        
        return services;
    }
}

