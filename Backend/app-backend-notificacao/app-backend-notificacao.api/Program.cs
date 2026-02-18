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

// Leitura de configurações globais
var appName = builder.Configuration["AppName"];
var pathBase = builder.Configuration["PathBase"];

// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

// Redirecionamento automático para Swagger
app.MapGet("/", (HttpContext context) => 
{
    var configPathBase = context.RequestServices.GetRequiredService<IConfiguration>()["PathBase"];
    var path = !string.IsNullOrWhiteSpace(configPathBase) ? $"{configPathBase}/swagger" : "/swagger";
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

