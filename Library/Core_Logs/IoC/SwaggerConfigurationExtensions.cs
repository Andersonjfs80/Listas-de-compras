using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Core_Logs.Filters;
using Core_Logs.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi;

namespace Core_Logs.IoC;

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddCoreSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var appName = configuration["AppName"] ?? "API";

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = appName, Version = "v1" });

            // 1. Filtro para Headers Obrigatórios
            c.OperationFilter<SwaggerHeaderFilter>();

            // 2. Configuração de Segurança JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCoreSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var appName = configuration["AppName"] ?? "API";
        var pathBase = configuration["PathBase"] ?? "";

        // Middleware para servir o CSS incorporado
        // Mapeamos para uma rota fixa /swagger-ui/custom.css que o browser resolverá via PathBase automaticamente se injetado corretamente
        app.Map("/swagger-ui/custom.css", builder =>
        {
            builder.Run(async context =>
            {
                context.Response.ContentType = "text/css";
                var assembly = Assembly.GetExecutingAssembly();
                
                // Tenta nomes de recursos comuns para garantir compatibilidade
                var resourceName = "Core_Logs.wwwroot.swagger-dark.css";
                using var stream = assembly.GetManifestResourceStream(resourceName);
                
                if (stream == null)
                {
                    // Fallback para listagem se falhar (ajuda no debug interno)
                    await context.Response.WriteAsync("/* CSS Resource not found */");
                    return;
                }

                using var reader = new StreamReader(stream);
                var css = await reader.ReadToEndAsync();
                await context.Response.WriteAsync(css);
            });
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", $"{appName} V1");
            c.RoutePrefix = "swagger";
            
            // Injeção do tema Dark usando caminho relativo para funcionar com PathBase e Gateway
            c.InjectStylesheet("../swagger-ui/custom.css");
        });

        return app;
    }
}
