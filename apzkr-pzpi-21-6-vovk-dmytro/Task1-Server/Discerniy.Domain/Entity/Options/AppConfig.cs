namespace Discerniy.Domain.Entity.Options
{
    public class AppConfig
    {
        public string InstanceName { get; set; } = default!;
        public UrlOptions Url { get; set; } = default!;
        public ConfirmationOptions Confirmation { get; set; } = default!;
        public RedisOption Redis { get; set; } = default!;
        public MongoDbOptions MongoDb { get; set; } = default!;
        public RabbitMqOptions RabbitMq { get; set; } = default!;
        public JwtOption Jwt { get; set; } = default!;
        public AuthServiceOption AuthService { get; set; } = default!;
        public RobotOptions Robot { get; set; } = default!;
        public EmailOption Email { get; set; } = default!;
    }
}
