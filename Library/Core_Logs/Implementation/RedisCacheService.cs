using System.Text.Json;
using Core_Logs.Configuration;
using Core_Logs.Interfaces;
using Core_Logs.Log;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace Core_Logs.Implementation;

/// <summary>
/// Implementação do serviço de cache usando Redis com integração de logs Kafka
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly IDatabase? _database;
    private readonly CacheSettings _settings;
    private readonly bool _isEnabled;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private ILogCustom? CurrentLog => _httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ILogCustom)) as ILogCustom;

    public RedisCacheService(IOptions<CacheSettings> settings, IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings.Value;
        _isEnabled = _settings.Enabled;
        _httpContextAccessor = httpContextAccessor;

        if (_isEnabled)
        {
            try
            {
                _redis = ConnectionMultiplexer.Connect(_settings.ConnectionString);
                _database = _redis.GetDatabase();
                
                LogToConsole($"[RedisCacheService] Conectado ao Redis: {_settings.ConnectionString}");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Erro ao conectar ao Redis: {ex.Message}. Cache desabilitado.";
                LogToConsole($"[RedisCacheService] {errorMsg}");
                _httpContextAccessor.HttpContext?.Response.Headers.Append("X-Cache-Error", "Redis connection failed");
                CurrentLog?.AdicionarLog("RedisCacheService", errorMsg, ex.StackTrace);
                _isEnabled = false;
            }
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _database == null)
            return null;

        var startTime = DateTime.UtcNow;
        
        try
        {
            LogToConsole($"[RedisCacheService] GET - Chave: {key}");
            
            var value = await _database.StringGetAsync(key);
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            
            if (!value.HasValue)
            {
                LogToConsole($"[RedisCacheService] GET - Cache MISS - Chave: {key} - Duração: {duration}ms");
                CurrentLog?.AdicionarLog(
                    "RedisCacheService.Get", 
                    $"Cache MISS - Chave: {key}", 
                    $"Duração: {duration}ms");
                return null;
            }

            var result = JsonSerializer.Deserialize<T>(value!);
            LogToConsole($"[RedisCacheService] GET - Cache HIT - Chave: {key} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog(
                "RedisCacheService.Get", 
                $"Cache HIT - Chave: {key}", 
                $"Duração: {duration}ms");
            
            return result;
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var errorMsg = $"Erro ao recuperar chave '{key}': {ex.Message}";
            LogToConsole($"[RedisCacheService] GET - ERRO - {errorMsg} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog("RedisCacheService.Get", errorMsg, ex.StackTrace);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _database == null)
            return;

        var startTime = DateTime.UtcNow;
        
        try
        {
            var ttl = expiration ?? TimeSpan.FromMinutes(_settings.DefaultTTLMinutes);
            LogToConsole($"[RedisCacheService] SET - Chave: {key} - TTL: {ttl.TotalMinutes}min");
            
            var serialized = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serialized, ttl);
            
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            LogToConsole($"[RedisCacheService] SET - Sucesso - Chave: {key} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog(
                "RedisCacheService.Set", 
                $"Cache SET - Chave: {key} - TTL: {ttl.TotalMinutes}min", 
                $"Duração: {duration}ms");
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var errorMsg = $"Erro ao armazenar chave '{key}': {ex.Message}";
            LogToConsole($"[RedisCacheService] SET - ERRO - {errorMsg} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog("RedisCacheService.Set", errorMsg, ex.StackTrace);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _database == null)
            return;

        var startTime = DateTime.UtcNow;
        
        try
        {
            LogToConsole($"[RedisCacheService] REMOVE - Chave: {key}");
            
            await _database.KeyDeleteAsync(key);
            
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            LogToConsole($"[RedisCacheService] REMOVE - Sucesso - Chave: {key} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog(
                "RedisCacheService.Remove", 
                $"Cache REMOVE - Chave: {key}", 
                $"Duração: {duration}ms");
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var errorMsg = $"Erro ao remover chave '{key}': {ex.Message}";
            LogToConsole($"[RedisCacheService] REMOVE - ERRO - {errorMsg} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog("RedisCacheService.Remove", errorMsg, ex.StackTrace);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _database == null)
            return false;

        var startTime = DateTime.UtcNow;
        
        try
        {
            LogToConsole($"[RedisCacheService] EXISTS - Chave: {key}");
            
            var exists = await _database.KeyExistsAsync(key);
            
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            LogToConsole($"[RedisCacheService] EXISTS - Resultado: {exists} - Chave: {key} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog(
                "RedisCacheService.Exists", 
                $"Cache EXISTS - Chave: {key} - Resultado: {exists}", 
                $"Duração: {duration}ms");
            
            return exists;
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var errorMsg = $"Erro ao verificar existência da chave '{key}': {ex.Message}";
            LogToConsole($"[RedisCacheService] EXISTS - ERRO - {errorMsg} - Duração: {duration}ms");
            CurrentLog?.AdicionarLog("RedisCacheService.Exists", errorMsg, ex.StackTrace);
            return false;
        }
    }

    private void LogToConsole(string message)
    {
        if (_settings.LogConsole)
        {
            Console.WriteLine(message);
        }
    }
}
