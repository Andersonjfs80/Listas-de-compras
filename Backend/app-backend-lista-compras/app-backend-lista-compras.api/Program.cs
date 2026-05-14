using Core_Logs.IoC;
using Core_Http.IoC;
using app_backend_lista_compras.infrastructure.IoC;
using app_backend_lista_compras.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);
builder.Services.AddCoreHttp<ListaComprasSettings>(builder.Configuration);

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

// 3. Inicialização Inteligente (Cria Banco/Tabelas se não existirem)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<app_backend_lista_compras.infrastructure.Configuration.AppDbContext>();
    db.Database.EnsureCreated();
}

// 4. Middlewares de Infraestrutura (Logs e Erros)
app.UseGlobalExceptionMiddleware();
app.UseKafkaLogging();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Homologation"))
{
    app.UseCoreSwagger(builder.Configuration);
}

// Redirecionamento automático para Swagger
app.MapGet("/", (HttpContext context) =>
{
    var configPathBase = context.RequestServices.GetRequiredService<IConfiguration>()["PathBase"];
    var path = !string.IsNullOrWhiteSpace(configPathBase) ? $"{configPathBase}/swagger" : "/swagger";
    return Results.Redirect(path);
});

app.UseAuthorization();

if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.MapGet($"{pathBase}/health", () => Results.Ok(new
    {
        appName,
        status = "ok",
        timestamp = DateTime.UtcNow
    }));
}

app.MapControllers();

app.Run();
