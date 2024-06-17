namespace Discerniy.Domain.Entity.Options
{
    public class JwtOption
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int UserExpiresInMinutes { get; set; }
        public int RobotExpiresInMinutes { get; set; }
        public int DeviceExpiresInMinutes { get; set; }
    }
}
