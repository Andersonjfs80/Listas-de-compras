using System.Threading.Tasks;

namespace Core_Logs.Interfaces
{
    public interface IKafkaLogger
    {
        Task LogAsync(string message);
        Task LogAsync<T>(T message);
    }
}
