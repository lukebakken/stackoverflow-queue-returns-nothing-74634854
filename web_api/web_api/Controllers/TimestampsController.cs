using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using web_api.Data;
using web_api.Models;
using web_api.Rabbit;

namespace web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimestampsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IRabbitMQConsumer _consumer;

        public TimestampsController(MyDbContext context, IRabbitMQConsumer consumer) 
        { 
            _context = context; 
            _consumer = consumer;
        }

        private async void CreateTimestamp(Timestamp timestampRequest)
        {
            timestampRequest.id = Guid.NewGuid();
            await _context.Timestamps.AddAsync(timestampRequest);
            await _context.SaveChangesAsync();
        }

        //public async Task<IActionResult> UpdateTimestamps(
        //  [FromBody] Timestamp timestampRequest)
        private void UpdateTimestamps(string timestampRequest)
        {
            Console.WriteLine($"update {timestampRequest}");
        }

        /*private static string GetMessage()
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
        }*/

        [HttpPost]
        public void RabbitMQConsumer()
        {

            string message = _consumer.ReceiveTimestamp();
            UpdateTimestamps(message);
            /*var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "consumption",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var consumer = new EventingBasicConsumer(channel);
            byte[] body;
            string message = null;
            consumer.Received += (model, ea) =>
            {
                body = ea.Body.ToArray();
                message = Encoding.UTF8.GetString(body);
            };

            if (message != null)
            {
                UpdateTimestamps(message);
            }

            channel.BasicConsume(queue: "consumption", autoAck: true, consumer: consumer);
            */
            //.FindAsync(timestampRequest.id);
        }

            //if (timestamp == null)
            //{
            //    CreateTimestamp(timestampRequest);
            //    return Ok("timestamp created");
            //}
            //else
            //{
            //    var device = await _context.Devices.FindAsync(timestampRequest.id);

            //    if (device == null)
            //    {
            //        return BadRequest();
            //    }

            //    if (device.max_consumption < timestamp.energy_consumption +
            //        timestampRequest.energy_consumption)
            //    {
            //        return BadRequest();
            //    }

            //    timestamp.deviceId = timestamp.deviceId;
            //    timestamp.timestamp = timestampRequest.timestamp;
            //    timestamp.energy_consumption += timestampRequest.energy_consumption;

            //    await _context.SaveChangesAsync();

            //    return Ok("energy consumption updated");
            //}
    }
}
