namespace Discerniy.Domain.Entity.Options
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public RabbitMqQueuesOption Queues { get; set; } = default!;
    }
}
