using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Entity.SubEntity
{
    public class GroupMemberModel
    {
        public string Id { get; set; }
        public ClientType Type { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public GroupMemberModel(IClient client)
        {
            Id = client.Id;
            Type = client.Type;
        }
    }
}
