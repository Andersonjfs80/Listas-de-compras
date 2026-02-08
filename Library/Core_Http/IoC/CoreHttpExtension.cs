using Core_Http.Interfaces;
using Core_Logs.Interfaces;
using Core_Http.Services;
using Core_Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core_Http.IoC;

public static class CoreHttpExtension
{
    public static IServiceCollection AddCoreHttp<TSettings>(this IServiceCollection services, IConfiguration configuration) 
        where TSettings : class, IGatewaySettings
    {
        // 1. Configurações Técnicas da Biblioteca (Core_Http_Settings)
        var libSection = configuration.GetSection(HttpConfig.SectionName);
        services.Configure<HttpConfig>(libSection);

        // 2. Configurações de Gateway (Mapeamento Híbrido: Global + Library)
        services.Configure<TSettings>(options => 
        {
            // Bind da seção específica do projeto (se existir)
            var sectionName = (typeof(TSettings).GetField("SectionName")?.GetValue(null) as string) ?? typeof(TSettings).Name;
            configuration.GetSection(sectionName).Bind(options);

            // Bind dos Identificadores GLOBAIS (Raiz)
            var appName = configuration["AppName"];
            var pathBase = configuration["PathBase"];

            var type = options.GetType();
            type.GetProperty("AppName")?.SetValue(options, appName ?? string.Empty);
            type.GetProperty("PathBase")?.SetValue(options, pathBase ?? string.Empty);

            // Bind dos Headers Globais (Vindo da tag da biblioteca)
            var globalHeaders = libSection.GetSection("GlobalHeaders").Get<List<GatewayHeaderConfig>>();
            if (globalHeaders != null)
            {
                type.GetProperty("GlobalHeaders")?.SetValue(options, globalHeaders);
            }
        });

        services.AddHttpContextAccessor();

        // Configuração otimizada para REUSO de conexões e suporte a DNS
        services.AddHttpClient("Core_Http_Proxy")
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                MaxConnectionsPerServer = 100
            });

        // SINGLETON para que o Circuit Breaker e o pool de conexões sejam compartilhados
        services.AddSingleton<IHttpProxyService, HttpProxyService>();
        services.AddSingleton<IHttpLogService, HttpLogService>();
        services.AddSingleton<IGenericHttpClient, GenericHttpClient>();
        
        return services;
    }
}
