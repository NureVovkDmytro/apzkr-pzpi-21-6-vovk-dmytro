namespace Discerniy.Domain.Entity.SubEntity
{
    public class ActionConfirmationModel
    {
        public string ActionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string UserId { get; set; }
        public object? Data { get; set; }
        public string Token { get; set; }

        public ActionConfirmationModel(string actionType, string userId, object? data = null)
        {
            ActionType = actionType;
            UserId = userId;
            Data = data;
            CreatedAt = DateTime.UtcNow;
            Token = GenerateToken();
        }

        protected string GenerateToken()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var token = Enumerable.Repeat(chars, 128)
                .Select(s => s[random.Next(s.Length)])
                .ToArray();
            return new string(token);
        }
    }
}
