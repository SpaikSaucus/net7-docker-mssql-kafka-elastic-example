using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UserPermission.Domain.Core;

namespace UserPermission.Infrastructure.Bootstrap.Extensions.ApplicationBuilder
{
    public static class KafkaHandlerBuilderExtensions
    {
        public static IApplicationBuilder UseKafkaHandlerMiddleware(this IApplicationBuilder builder, string topic)
        {
            return builder.UseMiddleware<KafkaHandlerMiddleware>(topic);
        }
    }

    public class KafkaHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IKafkaProducer producer;
        private readonly string topic;

        public KafkaHandlerMiddleware(RequestDelegate next, IKafkaProducer producer, string topic)
        {
            this.next = next;
            this.producer = producer;
            this.topic = topic;
        }

        public async Task Invoke(HttpContext context)
        {
            var messageDto = new
            {
                Id = Guid.NewGuid(),
                context.Request.Method,
                context.Request.Path,
            };
            var message = JsonConvert.SerializeObject(messageDto);

            _ = this.producer.ProduceAsync(this.topic, message);

            await this.next(context);
        }
    }
}
