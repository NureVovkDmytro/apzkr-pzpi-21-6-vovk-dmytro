namespace Discerniy.Domain.Responses
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public double ExpiresAt { get; set; }
        public double CreateAt { get; set; }

        public TokenResponse(string token, DateTime expiresAt, DateTime createAt)
        {
            Token = token;
            ExpiresAt = expiresAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            CreateAt = createAt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
