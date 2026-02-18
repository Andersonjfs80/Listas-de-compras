using Microsoft.Extensions.Options;
using Core_Http.Middlewares;
using Core_Logs.Filters;
using Core_Http.IoC;
using Core_Logs.IoC;
using app_api_produto.Configuration;
using app_api_produto.Endpoints;
using Core_Http.Gateway;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);
builder.Services.AddCoreHttp<ProdutoSettings>(builder.Configuration);

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCoreSwagger(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Leitura de configurações globais
var appName = builder.Configuration["AppName"];
var pathBase = builder.Configuration["PathBase"];

// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

// 2. Middlewares de Infraestrutura e Segurança
app.UseCors("AllowAll");
app.UseGlobalExceptionMiddleware();
app.UseKafkaLogging();
app.UseHeaderValidation(); 

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

// 3. Health check
app.MapGet($"{pathBase}/health", () => Results.Ok(new 
{ 
    appName,
    status = "gateway-ok", 
    pathBase,
    timestamp = DateTime.UtcNow 
}));

// 4. Grupo Raiz com Envelope Automático e Mapeamento
var apiGroup = app.MapGroup("")
                  .AddGatewayAutoEnvelope();

// Registro de endpoints do Gateway
apiGroup.MapEndpoints(new ProdutoEndpoint());

app.Run();

