using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Core_Logs.Constants;

namespace Core_Logs.Filters;

public class SwaggerHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        foreach (var headerName in StandardHeaderNames.MandatoryHeaders)
        {
            if (!operation.Parameters.Any(p => p.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = headerName,
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema { Type = "string" }
                });
            }
        }
    }
}
