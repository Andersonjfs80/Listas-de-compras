using System.Threading.Channels;
using Core_Logs.Interfaces;
using Core_Logs.Models;

namespace Core_Logs.Implementation;

/// <summary>
/// Implementação da fila de logs usando System.Threading.Channels.
/// </summary>
public class LogQueue : ILogQueue
{
    private readonly Channel<LogCustomModel> _channel;

    public LogQueue()
    {
        _channel = Channel.CreateUnbounded<LogCustomModel>(new UnboundedChannelOptions 
        { 
            SingleReader = true 
        });
    }

    public async ValueTask EnqueueAsync(LogCustomModel log)
    {
        await _channel.Writer.WriteAsync(log);
    }

    public IAsyncEnumerable<LogCustomModel> DequeueAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
