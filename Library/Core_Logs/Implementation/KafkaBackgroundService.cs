using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core_Logs.Interfaces;
using Core_Logs.Configuration;
using System.Text.Json;

namespace Core_Logs.Implementation;

/// <summary>
/// Serviço de background que processa a fila de logs e os envia ao Kafka.
/// </summary>
public class KafkaBackgroundService : BackgroundService
{
    private readonly ILogQueue _queue;
    private readonly IKafkaLogger _kafkaLogger;
    private readonly ILogger<KafkaBackgroundService> _logger;
    private readonly KafkaSettings _settings;

    public KafkaBackgroundService(ILogQueue queue, IKafkaLogger kafkaLogger, ILogger<KafkaBackgroundService> logger, IOptions<KafkaSettings> options)
    {
        _queue = queue;
        _kafkaLogger = kafkaLogger;
        _logger = logger;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de background do Kafka iniciado.");

        try
        {
            await foreach (var log in _queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    // 1. Envio para o Kafka
                    await _kafkaLogger.LogAsync(log);

                    // 2. Log opcional no Console (Debug/Desenvolvimento)
                    if (_settings.LogConsole)
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        var json = JsonSerializer.Serialize(log, options);
                        
                        Console.WriteLine("================ [KAFKA CONSOLIDATED LOG] ================");
                        Console.WriteLine(json);
                        Console.WriteLine("==========================================================");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar/enviar log consolidado.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Serviço de background do Kafka parando (cancelamento solicitado).");
        }
    }
}
