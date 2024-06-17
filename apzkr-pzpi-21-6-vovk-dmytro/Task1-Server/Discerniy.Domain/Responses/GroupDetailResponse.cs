using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Responses
{
    public class GroupDetailResponse : GroupResponse
    {
        public IList<GroupMemberModel> Members { get; set; } = new List<GroupMemberModel>();
        public IList<string> Admins { get; set; } = new List<string>();
        public GroupDetailResponse(GroupModel group) : base(group)
        {
            Members = group.Members.Values.ToList();
            Admins = group.Admins;
        }

        public static implicit operator GroupDetailResponse(GroupModel group)
        {
            return new GroupDetailResponse(group);
        }
    }
}
