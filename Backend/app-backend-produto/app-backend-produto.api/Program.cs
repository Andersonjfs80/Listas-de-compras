using Core_Logs.IoC;
using Core_Http.IoC;
using app_backend_produto.Configuration;
using app_backend_produto.infrastructure.IoC;
using app_backend_produto.infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);
builder.Services.AddCoreHttp<ProdutoSettings>(builder.Configuration);

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
    // 3. Inicialização Inteligente (Cria Banco/Tabelas se não existirem)
    app.InitializeDatabase();

// 2. Middlewares de Infraestrutura (Logs e Erros)
app.UseGlobalExceptionMiddleware(); // Captura erros e gera log de erro
app.UseKafkaLogging();              // Middleware principal de log consolidado

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

// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.MapGet($"{pathBase}/health", () => Results.Ok(new 
    { 
        appName,
        status = "gateway-ok", 
    }));
}

app.MapControllers();

app.Run();

