using Core_Logs.IoC;
using Core_Http.IoC;
using app_backend_produto.Configuration;
using app_backend_produto.infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);
builder.Services.AddCoreHttp<ProdutoSettings>(builder.Configuration);

// 2. Registro das Camadas da Aplicação
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Leitura de configurações globais
var appName = builder.Configuration["AppName"];
var pathBase = builder.Configuration["PathBase"];

// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

// 2. Middlewares de Infraestrutura (Logs e Erros)
app.UseGlobalExceptionMiddleware(); // Captura erros e gera log de erro
app.UseKafkaLogging();              // Middleware principal de log consolidado

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

