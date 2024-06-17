using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Responses
{
    public class MarkResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Icon { get; set; }
        public string UserId { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public GeoCoordinates Location { get; set; } = null!;
        public double Radius { get; set; }

        public MarkResponse(MarkModel mark)
        {
            Id = mark.Id;
            Name = mark.Name;
            Description = mark.Description;
            Icon = mark.Icon;
            UserId = mark.CreatedBy;
            GroupId = mark.GroupId;
            Location = new GeoCoordinates(mark.Location.Coordinates);
            Radius = mark.Radius;
        }

        public static implicit operator MarkResponse(MarkModel mark)
        {
            return new MarkResponse(mark);
        }
    }
}
