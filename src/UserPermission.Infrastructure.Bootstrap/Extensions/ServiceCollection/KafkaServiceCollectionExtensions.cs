using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ServiceCollection
{
    public class KafkaProducerConfig
    {
        private readonly IConfiguration configuration;
        private readonly IProducer<Null, string> producer;

        public KafkaProducerConfig(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.producer = this.BuildProducer();
        }

        public IProducer<Null, string> GetProducer()
        {
            return this.producer;
        }

        private IProducer<Null, string> BuildProducer()
        {
            var bootstrapServer = this.configuration.GetConnectionString("Kafka");
            var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServer };

            return new ProducerBuilder<Null, string>(producerConfig).Build();
        }
    }

    public static class KafkaServiceCollectionExtensions
    {
        public static void AddKafkaExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(provider => new KafkaProducerConfig(configuration));
            services.AddSingleton(provider => provider.GetRequiredService<KafkaProducerConfig>().GetProducer());
        }
    }
}
