using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserPermission.Domain.Core;

namespace UserPermission.Infrastructure.Services
{
    public class KafkaProducer : IKafkaProducer
    {
        const string messageOk = "Topic '{0}': Delivered '{1}' to '{2}'";
        const string messageError = "Topic '{0}': Delivery failed: {1}";
        private readonly IProducer<Null, string> producer;
        private readonly ILogger<KafkaProducer> logger;

        public KafkaProducer(IProducer<Null, string> producer, ILogger<KafkaProducer> logger) 
        { 
            this.logger = logger;
            this.producer = producer;
        }

        public async Task ProduceAsync(string topic, string message)
        {
            var msg = new Message<Null, string> { Value = message };

            try
            {
                var deliveryReport = await this.producer.ProduceAsync(topic, msg);
                this.logger.LogInformation(messageOk, topic, deliveryReport.Value, deliveryReport.TopicPartitionOffset);
            }
            catch (ProduceException<Null, string> e)
            {
                this.logger.LogError(messageError, topic, e.Error.Reason);
            }
        }
    }
}
