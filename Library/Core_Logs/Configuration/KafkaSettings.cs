using Confluent.Kafka;

namespace Core_Logs.Configuration
{
    public class KafkaSettings
    {
        public const string SectionName = "KafkaSettings";

        public string BootstrapServers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public bool LogAutomaticoHttp { get; set; } = true;
        public bool LogConsole { get; set; } = false;
        public SecurityProtocol? SecurityProtocol { get; set; } = Confluent.Kafka.SecurityProtocol.Plaintext;
        public SaslMechanism? SaslMechanism { get; set; }
        public string? SaslUsername { get; set; }
        public string? SaslPassword { get; set; }

        /// <summary>
        /// Configuração de campos que devem ser ofuscados no log.
        /// </summary>
        public ObfuscationSettings OfuscarCampos { get; set; } = new();
    }

    public class ObfuscationSettings
    {
        public List<string> Request { get; set; } = new();
        public List<string> Response { get; set; } = new();
    }
}
