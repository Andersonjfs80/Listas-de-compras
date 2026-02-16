using Core_Logs.Interfaces;
using Core_Logs.Security.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Core_Logs.Extensions;

public enum ParameterType
{
    Route,
    Query,
    Header
}

public record GatewayParameter(
    string Name, 
    ParameterType Type, 
    string? RenameTo = null, 
    string? Value = null,
    Func<object?, object?>? Transform = null);

/// <summary>
/// Objeto de contexto para manipulação manual da requisição via Hooks.
/// </summary>
public class GatewayRequestInfo
{
    public string Url { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; } = new();
}

internal class GatewayEndpointConfig
{
    public HttpMethod Verb { get; }
    public string RouteTemplate { get; }
    public List<GatewayParameter> Parameters { get; }
    public List<Action<HttpContext, object?, GatewayRequestInfo>> RequestHooks { get; } = new();
    public List<Func<object?, object?, object?>> ResponseTransforms { get; } = new();

    public GatewayEndpointConfig(HttpMethod verb, string routeTemplate, List<GatewayParameter> parameters)
    {
        Verb = verb;
        RouteTemplate = routeTemplate;
        Parameters = parameters;
    }
}

public class GatewayBuilder
{
    internal List<GatewayEndpointConfig> Endpoints { get; } = new();
    private GatewayEndpointConfig? _lastEndpoint;

    public GatewayBuilder Get(string routeTemplate, params GatewayParameter[] parameters)
    {
        _lastEndpoint = new GatewayEndpointConfig(HttpMethod.Get, routeTemplate, parameters.ToList());
        Endpoints.Add(_lastEndpoint);
        return this;
    }

    public GatewayBuilder Post(string routeTemplate, params GatewayParameter[] parameters)
    {
        _lastEndpoint = new GatewayEndpointConfig(HttpMethod.Post, routeTemplate, parameters.ToList());
        Endpoints.Add(_lastEndpoint);
        return this;
    }

    public GatewayBuilder Put(string routeTemplate, params GatewayParameter[] parameters)
    {
        _lastEndpoint = new GatewayEndpointConfig(HttpMethod.Put, routeTemplate, parameters.ToList());
        Endpoints.Add(_lastEndpoint);
        return this;
    }

    public GatewayBuilder Delete(string routeTemplate, params GatewayParameter[] parameters)
    {
        _lastEndpoint = new GatewayEndpointConfig(HttpMethod.Delete, routeTemplate, parameters.ToList());
        Endpoints.Add(_lastEndpoint);
        return this;
    }

    /// <summary>
    /// Adiciona um hook para manipular a requisição antes do envio (Ex: injetar headers do token).
    /// </summary>
    public GatewayBuilder WithRequestHook(Action<HttpContext, object?, GatewayRequestInfo> hook)
    {
        _lastEndpoint?.RequestHooks.Add(hook);
        return this;
    }

    /// <summary>
    /// Adiciona um transformer para alterar o JSON de resposta antes de devolver ao cliente.
    /// </summary>
    public GatewayBuilder WithResponseTransform(Func<object?, object?, object?> transform)
    {
        _lastEndpoint?.ResponseTransforms.Add(transform);
        return this;
    }
}

public static class GatewayExtensions
{
    /// <summary>
    /// Mapeia um Gateway de forma genérica para um serviço específico.
    /// </summary>
    /// <typeparam name="TSettings">Tipo da classe de configuração (implementa IGatewaySettings)</typeparam>
    /// <param name="group">Grupo de rotas do ASP.NET Core</param>
    /// <param name="serviceName">Nome do serviço (usado na construção da URL backend)</param>
    /// <param name="urlSelector">Seletor para obter a URL base do serviço a partir das configurações</param>
    /// <param name="configure">Ação de configuração dos endpoints</param>
    public static void MapGateway<TSettings>(
        this RouteGroupBuilder group,
        string serviceName,
        Func<TSettings, string> urlSelector,
        Action<GatewayBuilder> configure) where TSettings : class, IGatewaySettings
    {
        if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("Service Name required");

        var builder = new GatewayBuilder();
        configure(builder);

        if (builder.Endpoints.Count == 0)
        {
            throw new InvalidOperationException($"Nenhum endpoint definido para '{serviceName}'.");
        }

        foreach (var endpoint in builder.Endpoints)
        {
            ValidateEndpointConfig(endpoint, serviceName);
            MapEndpoint(group, endpoint, serviceName, urlSelector);
        }
    }

    private static void ValidateEndpointConfig(GatewayEndpointConfig endpoint, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(endpoint.RouteTemplate)) 
            throw new InvalidOperationException($"RouteTemplate vazio para {serviceName}");
        
        if (!endpoint.RouteTemplate.StartsWith("/")) 
            throw new InvalidOperationException($"RouteTemplate deve começar com '/' em {serviceName}");

        if (endpoint.Verb == HttpMethod.Delete && endpoint.RouteTemplate == "/")
            throw new InvalidOperationException($"DELETE na raiz não permitido em {serviceName}");

        // Validação de Integridade: Parâmetros de Rota devem existir no Template (Forma A)
        foreach (var param in endpoint.Parameters.Where(p => p.Type == ParameterType.Route))
        {
            var patternRegex = new Regex($"\\{{{param.Name}(:[^}}]+)?\\}}", RegexOptions.IgnoreCase);
            if (!patternRegex.IsMatch(endpoint.RouteTemplate))
            {
                throw new InvalidOperationException(
                    $"Erro de Configuração em '{serviceName}': O parâmetro de rota '{param.Name}' foi definido, " +
                    $"mas não foi encontrado no template '{endpoint.RouteTemplate}'.");
            }
        }

        // Validação de Integridade: Placeholders na Rota devem estar configurados (Forma B)
        var placeholders = Regex.Matches(endpoint.RouteTemplate, @"\{([^:|}]+)(:[^}]+)?\}")
                                .Select(m => m.Groups[1].Value)
                                .ToList();

        foreach (var placeholder in placeholders)
        {
            if (!endpoint.Parameters.Any(p => p.Name.Equals(placeholder, StringComparison.OrdinalIgnoreCase) && p.Type == ParameterType.Route))
            {
                throw new InvalidOperationException(
                    $"Erro de Configuração em '{serviceName}': O placeholder '{{{placeholder}}}' foi encontrado na rota, " +
                    $"mas não existe um GatewayParameter correspondente do tipo Route.");
            }
        }
    }

    private static void MapEndpoint<TSettings>(
        RouteGroupBuilder group,
        GatewayEndpointConfig endpoint,
        string serviceName,
        Func<TSettings, string> urlSelector) where TSettings : class, IGatewaySettings
    {
        var verb = endpoint.Verb;
        var routeTemplate = endpoint.RouteTemplate;
        var parameters = endpoint.Parameters;

        async Task<IResult> HandleRequest(
            IGenericHttpClient client, 
            IHttpContextAccessor contextAccessor,
            ITokenService tokenService,
            TSettings settings, 
            JsonElement? body = null)
        {
            var ctx = contextAccessor.HttpContext;
            if (ctx == null) return Results.Problem("Contexto HTTP não disponível.");

            // Validação Body para POST e PUT
            if ((verb == HttpMethod.Post || verb == HttpMethod.Put) && 
                (!body.HasValue || body.Value.ValueKind == JsonValueKind.Null))
            {
                return Results.BadRequest(new { error = "Body JSON obrigatório." });
            }

            try
            {
                // Identificadores Obrigatórios
                if (string.IsNullOrWhiteSpace(settings.AppName) || string.IsNullOrWhiteSpace(settings.PathBase))
                {
                    throw new InvalidOperationException("Configurações AppName/PathBase não encontradas.");
                }

                var baseUrl = urlSelector(settings);
                if (string.IsNullOrEmpty(baseUrl)) throw new InvalidOperationException($"URL Base para '{serviceName}' não configurada.");

                // Resolver Sessão (para uso em Hooks e Transformers)
                string token = string.Empty;
                if (ctx.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    token = authHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                }

                var session = !string.IsNullOrEmpty(token) ? tokenService.GetSession(token) : null;

                var headerValues = new Dictionary<string, string>();

                // --- 1. Processar Headers Globais ---
                if (settings.GlobalHeaders != null)
                {
                    foreach (var hConfig in settings.GlobalHeaders)
                    {
                        if (ctx.Request.Headers.TryGetValue(hConfig.Name, out var hVal))
                            headerValues[hConfig.Name] = hVal.ToString();
                        else if (hConfig.Required)
                            return Results.BadRequest(new { error = $"Header global '{hConfig.Name}' obrigatório." });
                    }
                }

                // --- 1.1 Processar e VALIDAR Headers Padrão (Automatic Forwarding) ---
                // O Core Logs verifica se os headers obrigatórios estão presentes. Se não estiverem, retorna Erro.
                foreach (var headerName in Core_Logs.Constants.StandardHeaderNames.MandatoryHeaders)
                {
                     if (ctx.Request.Headers.TryGetValue(headerName, out var hVal))
                     {
                          headerValues[headerName] = hVal.ToString();
                     }
                     else
                     {
                          return Results.BadRequest(new { error = $"Header padrão obrigatório '{headerName}' ausente." });
                     }
                }

                // --- 2. Processar Parâmetros do Endpoint (com Transformers) ---
                var routeValues = new Dictionary<string, string>();
                var queryValues = new Dictionary<string, string>();

                foreach (var param in parameters)
                {
                    object? value = null;
                    
                    if (!string.IsNullOrEmpty(param.Value)) value = param.Value;
                    else
                    {
                        if (param.Type == ParameterType.Route && ctx.Request.RouteValues.TryGetValue(param.Name, out var rv)) value = rv;
                        else if (param.Type == ParameterType.Query && ctx.Request.Query.TryGetValue(param.Name, out var qv)) value = qv.ToString();
                        else if (param.Type == ParameterType.Header && ctx.Request.Headers.TryGetValue(param.Name, out var hv)) value = hv.ToString();
                    }

                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        return Results.BadRequest(new { error = $"Campo '{param.Name}' ({param.Type}) obrigatório." });

                    // APLICAR TRANSFORMER DE ENTRADA
                    if (param.Transform != null) value = param.Transform(value);

                    var targetName = param.RenameTo ?? param.Name;
                    var finalValue = value?.ToString() ?? string.Empty;

                    if (param.Type == ParameterType.Route) routeValues[targetName] = finalValue;
                    else if (param.Type == ParameterType.Query) queryValues[targetName] = finalValue;
                    else if (param.Type == ParameterType.Header) headerValues[targetName] = finalValue;
                }

                // --- 3. Construir URL e GatewayRequestInfo ---
                var relativePath = routeTemplate;
                foreach(var kvp in routeValues)
                    relativePath = Regex.Replace(relativePath, $"\\{{{kvp.Key}(:[^}}]+)?\\}}", kvp.Value, RegexOptions.IgnoreCase);
                
                var backendPath = $"{settings.PathBase.TrimEnd('/')}/{serviceName}{relativePath}";

                // Contexto para Hooks
                var requestInfo = new GatewayRequestInfo { Url = backendPath };
                foreach(var h in headerValues) requestInfo.Headers[h.Key] = h.Value;

                // --- 4. EXECUTAR REQUEST HOOKS (Oportunidade de manipular Headers/URL com dados da Sessão) ---
                foreach(var hook in endpoint.RequestHooks) hook(ctx, session, requestInfo);

                // Re-append Query String após hooks
                var finalUrl = relativePath;
                if (queryValues.Count > 0)
                {
                    var qs = string.Join("&", queryValues.Select(k => $"{k.Key}={Uri.EscapeDataString(k.Value)}"));
                    finalUrl += finalUrl.Contains("?") ? $"&{qs}" : $"?{qs}";
                }

                // --- 5. Executar chamada ao backend ---
                object? backendResponse = null;
                if (verb == HttpMethod.Get) backendResponse = await client.GetAsync<object>(baseUrl, finalUrl, requestInfo.Headers, default);
                else if (verb == HttpMethod.Post) backendResponse = await client.PostAsync<object?, object>(baseUrl, finalUrl, body, requestInfo.Headers, default);
                else if (verb == HttpMethod.Put) backendResponse = await client.PutAsync<object?, object>(baseUrl, finalUrl, body, requestInfo.Headers, default);
                else if (verb == HttpMethod.Delete) backendResponse = await client.DeleteAsync<object>(baseUrl, finalUrl, requestInfo.Headers, default);

                if (backendResponse == null) return Results.StatusCode(405);

                // --- 6. EXECUTAR RESPONSE TRANSFORMS ---
                var finalResponse = backendResponse;
                foreach(var transform in endpoint.ResponseTransforms)
                {
                    finalResponse = transform(finalResponse, session);
                }

                return Results.Ok(finalResponse);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        if (verb == HttpMethod.Get) 
            group.MapGet(routeTemplate, (IGenericHttpClient client, IHttpContextAccessor contextAccessor, ITokenService tokenService, IOptions<TSettings> options) 
                => HandleRequest(client, contextAccessor, tokenService, options.Value));
        
        else if (verb == HttpMethod.Post) 
            group.MapPost(routeTemplate, (IGenericHttpClient client, IHttpContextAccessor contextAccessor, ITokenService tokenService, IOptions<TSettings> options, [FromBody] JsonElement? body) 
                => HandleRequest(client, contextAccessor, tokenService, options.Value, body));
        
        else if (verb == HttpMethod.Put) 
            group.MapPut(routeTemplate, (IGenericHttpClient client, IHttpContextAccessor contextAccessor, ITokenService tokenService, IOptions<TSettings> options, [FromBody] JsonElement? body) 
                => HandleRequest(client, contextAccessor, tokenService, options.Value, body));
        
        else if (verb == HttpMethod.Delete) 
            group.MapDelete(routeTemplate, (IGenericHttpClient client, IHttpContextAccessor contextAccessor, ITokenService tokenService, IOptions<TSettings> options) 
                => HandleRequest(client, contextAccessor, tokenService, options.Value));
    }
}
