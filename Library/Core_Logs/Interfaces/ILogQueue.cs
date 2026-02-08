using Core_Logs.Models;

namespace Core_Logs.Interfaces;

/// <summary>
/// Interface para fila de logs assíncrona.
/// </summary>
public interface ILogQueue
{
    ValueTask EnqueueAsync(LogCustomModel log);
    IAsyncEnumerable<LogCustomModel> DequeueAllAsync(CancellationToken cancellationToken);
}
