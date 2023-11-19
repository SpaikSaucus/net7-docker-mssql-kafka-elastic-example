using System.Threading.Tasks;

namespace UserPermission.Domain.Core
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string msg);
    }
}
