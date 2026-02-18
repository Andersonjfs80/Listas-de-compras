using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Core_Logs.Security.Models;
using Core_Logs.IoC;
using Core_Http.IoC;
using app_backend_autenticacao.infrastructure.IoC;
using Microsoft.EntityFrameworkCore;
using Core_Logs.Interfaces;
using app_backend_autenticacao.api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro das Bibliotecas Core
builder.Services.AddCoreLogs(builder.Configuration);
builder.Services.AddCoreHttp<AutenticacaoSettings>(builder.Configuration);

// 2. Configuração de Autenticação
var securitySettings = builder.Configuration.GetSection(SecuritySettings.SectionName).Get<SecuritySettings>() ?? new SecuritySettings();
var key = Encoding.ASCII.GetBytes(string.IsNullOrEmpty(securitySettings.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : securitySettings.SecretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// 3. Registro das Camadas da Aplicação
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logCustom = services.GetRequiredService<ILogCustom>();
    var context = services.GetRequiredService<app_backend_autenticacao.infrastructure.Configuration.AppDbContext>();
    
    int retries = 10;
    while (retries > 0)
    {
        try
        {
            logCustom.AdicionarLog("AutenticacaoBackend: Tentando aplicar migrações...");
            context.Database.Migrate();
            logCustom.AdicionarLog("AutenticacaoBackend: Migrações aplicadas com sucesso!");
            
            var securityService = services.GetRequiredService<ISecurityService>();
            if (!app.Environment.IsProduction())
            {
                app_backend_autenticacao.infrastructure.Configuration.DbInitializer.Seed(context, securityService);
            }
            
            await logCustom.EnviarLogAsync();
            break;
        }
        catch (Exception ex)
        {
            logCustom.AdicionarErro($"AuthBackend: Falha ao migrar banco. Retries: {retries}", ex);
            await logCustom.EnviarLogAsync();
            
            retries--;
            if (retries == 0) throw;
            System.Threading.Thread.Sleep(5000);
        }
    }
}

// 3. Middlewares de Infraestrutura (Logs e Erros)
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


// Configuração de Prefixo da API (PathBase)
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.MapGet($"{pathBase}/health", () => Results.Ok(new 
    { 
        appName,
        status = "gateway-ok", 
    }));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

