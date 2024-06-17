namespace Discerniy.Domain.Entity.Options
{
    public class CorsServerOptions
    {
        public string Name { get; set; } = null!;
        public string[] AllowedMethors { get; set; } = null!;
        public string[] AllowedOrigins { get; set; } = null!;
        public string[]? AllowedHeaders { get; set; }
        public bool? AllowCredentials { get; set; }
    }
}
