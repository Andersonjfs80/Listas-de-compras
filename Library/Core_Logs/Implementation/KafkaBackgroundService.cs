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
public class KafkaBackgroundService(
    ILogQueue queue,
    IKafkaLogger kafkaLogger,
    ILogger<KafkaBackgroundService> logger,
    IOptions<KafkaSettings> options) : BackgroundService
{
    private readonly ILogQueue _queue = queue;
    private readonly IKafkaLogger _kafkaLogger = kafkaLogger;
    private readonly ILogger<KafkaBackgroundService> _logger = logger;
    private readonly KafkaSettings _settings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Serviço de background do Kafka iniciado. Kafka Habilitado: {_settings.Enabled}");

        try
        {
            await foreach (var log in _queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    // Envio direto para o Kafka se habilitado
                    if (_settings.Enabled)
                    {
                        await _kafkaLogger.LogAsync(log);
                    }

                    // Log opcional no Console (Debug/Desenvolvimento)
                    if (_settings.LogConsole)
                    {
                        if (!_settings.Enabled)
                        {
                            Console.WriteLine("[KAFKA DISABLED] Log captured but NOT sent to Kafka:");
                        }

                        var opt = new JsonSerializerOptions { WriteIndented = true };
                        var json = JsonSerializer.Serialize(log, opt);
                        // Console.WriteLine(json);
                    }
                }
                catch (Exception ex)
                {
                    if (_settings.Enabled)
                    {
                        _logger.LogError(ex, "Erro ao enviar log consolidado para Kafka.");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Serviço de background do Kafka parando (cancelamento solicitado).");
        }
    }
}
