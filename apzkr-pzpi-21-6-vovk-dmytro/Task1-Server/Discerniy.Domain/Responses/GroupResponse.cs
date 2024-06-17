using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Responses
{
    public class GroupResponse
    {

        public string Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = "";
        public int AccessLevel { get; set; } = 0;

        public int MemberCount { get; set; } = 0;

        public IReadOnlyCollection<GeoCoordinates>? ResponsibilityArea { get; set; } = null;

        public GroupResponse(GroupModel group)
        {
            Id = group.Id;
            CreatedAt = group.CreatedAt;
            Name = group.Name;
            Description = group.Description;
            AccessLevel = group.AccessLevel;
            ResponsibilityArea = group.ResponsibilityArea?.Coordinates.Exterior.Positions.Select(x => new GeoCoordinates(x)).ToList();
            MemberCount = group.Members.Count;
        }

        public static implicit operator GroupResponse(GroupModel group)
        {
            return new GroupResponse(group);
        }
    }
}
