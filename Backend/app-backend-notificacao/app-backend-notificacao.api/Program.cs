using app_backend_notificacao.infrastructure.IoC;
using Core_Logs.IoC;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);

// 2. Registro das Camadas da Aplicação
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCoreSwagger(builder.Configuration);

var app = builder.Build();

// ... (resto do código)

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Homologation"))
{
    app.UseCoreSwagger(builder.Configuration);
}

// Redirecionamento automático para Swagger
app.MapGet("/", (HttpContext context) => 
{
    var path = context.Request.PathBase.HasValue ? $"{context.Request.PathBase}/swagger" : "/swagger";
    return Results.Redirect(path);
});




// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.MapGet($"{pathBase}/health", () => Results.Ok(new 
    { 
        appName,
        status = "gateway-ok", 
    }));
}

app.UseAuthorization();
app.MapControllers();

app.Run();

