using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Responses
{
    public class LocationResponse : GeoCoordinates
    {
        public string Id { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public double Compass { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public LocationResponse(IClient client) : base(client.Location?.Coordinates?.Easting ?? 0, client.Location?.Coordinates?.Northing ?? 0)
        {
            Id = client.Id;
            Nickname = client.Nickname;
            Compass = client.Compass;
        }

        public LocationResponse()
        {
        }
    }
}
