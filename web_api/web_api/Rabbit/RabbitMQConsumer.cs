using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Diagnostics;

namespace web_api.Rabbit
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private string _lastMessage = null;
        private ulong _lastDeliveryTag = 0;

        private bool disposedValue;

        public RabbitMQConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "consumption",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            /*
             * Note: only one message will be consumed at a time.
             */
            _channel.BasicQos(0, 1, false);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ReceivedHandler;
        }

        public string ReceiveTimestamp()
        {
            try
            {
                _semaphore.Wait();
                if (_lastDeliveryTag > 0)
                {
                    _channel.BasicAck(_lastDeliveryTag, false);
                    _lastDeliveryTag = 0;
                    return _lastMessage;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    string consumerTag = _consumer.ConsumerTags.First();
                    _channel.BasicCancel(consumerTag);
                    _channel.Dispose();
                    _connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void ReceivedHandler(object model, BasicDeliverEventArgs ea)
        {
            Debug.Assert(Object.ReferenceEquals(model, ea));
            try
            {
                var body = ea.Body.ToArray();
                _semaphore.Wait();
                _lastMessage = Encoding.UTF8.GetString(body);
                _lastDeliveryTag = ea.DeliveryTag;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
