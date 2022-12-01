using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName= "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "consumption",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Guid id = Guid.NewGuid();
Guid deviceId = new("43215a86-47af-4ccb-3832-08dabf2b75a9");
DateTime timestamp = DateTime.UtcNow;

var message = new
{
    id = id,
    deviceId = deviceId,
    timestamp = timestamp,
    energy_consumption = 1
};

var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

channel.BasicPublish("", "consumption", null, body);


//namespace RabbitMQ_Producer
//{
//    static class Program
//    {
//        static void Main(string[] args)
//        {
//            var factory = new ConnectionFactory
//            {
//                Uri = new("amqp://guest:guest@localhost:5672")
//            };

//            using var connection = factory.CreateConnection();
//            using var channel = connection.CreateModel();
//            channel.QueueDeclare("comsumption-timestamp",
//                durable: true,
//                exclusive: false,
//                autoDelete: false,
//                arguments: null
//                );

//            Guid id = Guid.NewGuid();
//            Guid deviceId = new("43215a86-47af-4ccb-3832-08dabf2b75a9");
//            DateTime timestamp = DateTime.UtcNow;

//            var message = new
//            {
//                id = id,
//                deviceId = deviceId,
//                timestamp = timestamp,
//                energy_consumption = 0
//            };
//            var body = Encoding.UTF8.GetBytes(JsonConvert.
//                SerializeObject(message));

//            channel.BasicPublish("", "consumption-timestamp", null, body);
//        }
//    }
//}