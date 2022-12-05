namespace web_api.Rabbit
{
    public interface IRabbitMQConsumer : IDisposable
    {
        public string ReceiveTimestamp();
    }
}
