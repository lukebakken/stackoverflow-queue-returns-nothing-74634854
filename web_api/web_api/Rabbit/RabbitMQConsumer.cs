using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace web_api.Rabbit
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        public string ReceiveTimestamp()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: "consumption",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var consumer = new EventingBasicConsumer(channel);

            string message = string.Empty;
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                message = Encoding.UTF8.GetString(body);
            };

            channel.BasicConsume(queue: "consumption",
                autoAck: true,
                consumer: consumer
                );
            return message;
        }

    }
}
