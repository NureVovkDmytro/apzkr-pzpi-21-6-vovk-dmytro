namespace Discerniy.Domain.Entity.SubEntity
{
    public class ClientSession
    {
        public string Id { get; set; } = null!;
        public DateTime LastUpdated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
