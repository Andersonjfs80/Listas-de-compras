using Core_Logs.Constants;
using Core_Logs.Gateway;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace Core_Http.Middlewares;

public class HeaderValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Ignorar validação para Health Check ou Swagger (opcional)
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && (path.Contains("/health") || path.Contains("/swagger")))
        {
            await next(context);
            return;
        }

        var missingHeaders = StandardHeaderNames.MandatoryHeaders
            .Where(h => !context.Request.Headers.ContainsKey(h))
            .ToList();

        if (missingHeaders.Any())
        {
            var result = GatewayResponse.Error(
                message: "Cabeçalhos obrigatórios ausentes: " + string.Join(", ", missingHeaders),
                statusCode: HttpStatusCode.BadRequest,
                code: "MISSING_MANDATORY_HEADERS",
                details: "Certifique-se de que os headers obrigatórios foram enviados corretamente."
            );

            await result.ExecuteAsync(context);
            return;
        }

        await next(context);
    }
}

public static class HeaderValidationExtensions
{
    public static IApplicationBuilder UseHeaderValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HeaderValidationMiddleware>();
    }
}
