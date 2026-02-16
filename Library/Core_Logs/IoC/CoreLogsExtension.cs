using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core_Logs.Log;
using Core_Logs.Middlewares;
using Core_Logs.Configuration;
using Core_Logs.Interfaces;
using Core_Logs.Implementation;
using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using Core_Logs.Security.Services;
using Core_Logs.Models;

namespace Core_Logs.IoC;

public static class CoreLogsExtension
{
    public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static IApplicationBuilder UseKafkaLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<KafkaLoggingMiddleware>();
    }
    
    public static IServiceCollection AddCoreLogs(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Inicializar Metadados de Infraestrutura diretamente no LogCustomModel
        LogCustomModel.GlobalAppName = configuration["AppName"] ?? string.Empty;
        

        // Resolução de PodName (Cloud/Kubernetes)
        var resolvedPod = configuration["HOSTNAME"] ?? configuration["POD_NAME"];
        if (!string.IsNullOrEmpty(resolvedPod))
            LogCustomModel.GlobalPodName = resolvedPod;

        // 2. Registro da Configuração (Seção KafkaSettings)
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));
        
        // 3. Registro do Logger
        services.AddSingleton<IKafkaLogger, KafkaLogger>();

        // 4. CONFIGURAÇÃO DE SEGURANÇA (TOKENS)
        var securitySettingsSection = configuration.GetSection(SecuritySettings.SectionName);
        services.Configure<SecuritySettings>(securitySettingsSection);
        var securitySettings = securitySettingsSection.Get<SecuritySettings>() ?? new SecuritySettings();

        if (securitySettings.TokenProvider == TokenProviderType.JOSE)
        {
            services.AddSingleton<ITokenService, JoseTokenService>();
        }
        else
        {
            services.AddSingleton<ITokenService, JwtTokenService>();
        }

        // 4.1 NOVO MOTOR DE SEGURANÇA (CRIPTO/HASH)
        services.AddSingleton<ISecurityService, SecurityService>();
        
        // 5. INFRAESTRUTURA DE LOG CONSOLIDADO

        services.AddSingleton<ILogQueue, LogQueue>();
        services.AddHostedService<KafkaBackgroundService>();
        
        // 6. CACHE REDIS
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        // 7. LOG CUSTOMIZADO
        services.AddScoped<ILogCustom, LogCustom>();

        // 8. CONTEXTO DE USUÁRIO
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        
        return services;
    }
}
