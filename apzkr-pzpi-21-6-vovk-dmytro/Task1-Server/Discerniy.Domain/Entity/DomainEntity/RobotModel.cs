using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using System.ComponentModel.DataAnnotations;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Entity.DomainEntity
{
    public class RobotModel : IIdentifier, IClient
    {
        [Required]
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        public string Nickname { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastOnline { get; set; }
        public bool TokenAutoRefresh { get; set; }

        public GeoJsonPoint<GeoJson2DProjectedCoordinates> Location { get; set; }
        [BsonRepresentation(BsonType.Double)]
        public double Compass { get; set; }

        public ClientType Type => ClientType.Robot;

        public string Key { get; set; } = null!;

        public ClientStatus Status { get; set; }
        public int ScanRadius { get; set; }
        public int UpdateLocationSecondsInterval { get; set; } = 10;
        public IList<ClientSession> Sessions { get; set; } = new List<ClientSession>();
        public string GroupId { get; set; } = null!;
        public int AccessLevel { get; set; }
        public IList<string> WebSocketConections { get; set; } = new List<string>();
    }
}
