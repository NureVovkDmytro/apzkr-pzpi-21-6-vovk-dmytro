namespace Discerniy.Domain.Responses
{
    public class DeviceTokenResponse : TokenResponse
    {
        public int UpdateInterval { get; set; }
        public DeviceTokenResponse(string token, DateTime expiresAt, DateTime createAt, int updateInterval) : base(token, expiresAt, createAt)
        {
            UpdateInterval = updateInterval;
        }
    }
}
