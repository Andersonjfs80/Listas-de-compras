using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core_Logs.Interfaces;

/// <summary>
/// Interface para cliente HTTP genérico.
/// Movida de Core_Http para evitar dependência circular.
/// </summary>
public interface IGenericHttpClient
{
    Task<TResponse?> GetAsync<TResponse>(string baseUrl, string path, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string baseUrl, string path, TRequest body, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string baseUrl, string path, TRequest body, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default);
    Task<TResponse?> DeleteAsync<TResponse>(string baseUrl, string path, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default);
}
