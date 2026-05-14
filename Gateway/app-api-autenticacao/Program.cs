using Microsoft.Extensions.Options;
using Core_Http.Middlewares;
using Core_Logs.Filters;
using Core_Http.IoC;
using Core_Logs.IoC;
using app_api_autenticacao.Configuration;
using app_api_autenticacao.Endpoints;
using Core_Http.Gateway;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);
builder.Services.AddCoreHttp<AutenticacaoSettings>(builder.Configuration);

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

app.UseCors("AllowAll");

// 1. O PRIMEIRO MIDDLEWARE DEVE SER A DESCRIPTOGRAFIA
app.UseBodyEncryptionMiddleware();

// Leitura de configurações globais
var appName = builder.Configuration["AppName"];
var pathBase = builder.Configuration["PathBase"];

// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}
app.UseGlobalExceptionMiddleware();
app.UseKafkaLogging();
app.UseHeaderValidation(); 

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Homologation"))
{
    app.UseCoreSwagger(builder.Configuration);
}

// Redirecionamento automático para Swagger na raiz do PathBase
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
    status = "gateway-auth-ok", 
    pathBase,
    timestamp = DateTime.UtcNow 
}));

// 4. Grupo Raiz com Envelope Automático e Mapeamento
var apiGroup = app.MapGroup("")
                  .AddGatewayAutoEnvelope();

// Registro de endpoints do Gateway
apiGroup.MapEndpoints(new AutenticacaoEndpoint());

// 2. Endpoint OFICIAL de Ingestão de Logs do Frontend (Suporta Batching)
apiGroup.MapPost("/logs", async (System.Text.Json.JsonElement body, Core_Logs.Interfaces.ILogQueue queue) => 
{
    if (body.ValueKind == System.Text.Json.JsonValueKind.Array)
    {
        var logs = System.Text.Json.JsonSerializer.Deserialize<List<Core_Logs.Models.LogCustomModel>>(body.GetRawText());
        if (logs != null)
        {
            foreach (var log in logs)
            {
                log.Timestamp = log.Timestamp == default ? DateTime.UtcNow : log.Timestamp;
                await queue.EnqueueAsync(log);
            }
        }
    }
    else
    {
        var log = System.Text.Json.JsonSerializer.Deserialize<Core_Logs.Models.LogCustomModel>(body.GetRawText());
        if (log != null)
        {
            log.Timestamp = log.Timestamp == default ? DateTime.UtcNow : log.Timestamp;
            await queue.EnqueueAsync(log);
        }
    }
    
    return Results.Accepted();
});

app.Run();
