using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace app_backend_seguranca.infrastructure.Mappings;

public static class MapsterConfiguration
{
    public static void AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        var assemblies = new[] { Assembly.GetExecutingAssembly() };
        config.Scan(assemblies);
    }
}
