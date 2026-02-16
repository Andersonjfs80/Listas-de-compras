using Confluent.Kafka;
using Core_Logs.Configuration;
using Core_Logs.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Core_Logs.Implementation
{
    public class KafkaLogger : IKafkaLogger, IDisposable
    {
        private readonly IProducer<string, string>? _producer;
        private readonly string? _topic;
        private readonly bool _enabled;

        public KafkaLogger(IOptions<KafkaSettings> options)
        {
            var config = options.Value;
            _enabled = config.Enabled;

            if (!_enabled) return;

            if (string.IsNullOrWhiteSpace(config.BootstrapServers))
                throw new ArgumentException("A configuração 'BootstrapServers' é obrigatória para o Kafka.", nameof(config.BootstrapServers));

            if (string.IsNullOrWhiteSpace(config.Topic))
                throw new ArgumentException("A configuração 'Topic' é obrigatória para o Kafka.", nameof(config.Topic));

            _topic = config.Topic;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config.BootstrapServers,
                SecurityProtocol = config.SecurityProtocol,
                SaslMechanism = config.SaslMechanism,
                SaslUsername = config.SaslUsername,
                SaslPassword = config.SaslPassword,
                MessageMaxBytes = config.MessageMaxBytes
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task LogAsync(string message)
        {
            if (!_enabled || _producer == null) return;

            await _producer.ProduceAsync(_topic!, new Message<string, string> 
            { 
                Key = Guid.NewGuid().ToString(), 
                Value = message 
            });
        }

        public async Task LogAsync<T>(T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            await LogAsync(jsonMessage);
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}
