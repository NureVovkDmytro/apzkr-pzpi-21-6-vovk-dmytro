using Discerniy.Domain.Interface.Entity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Discerniy.Domain.Entity.DomainEntity
{
    public class MarkModel : IIdentifier
    {
        [BsonId]
        public string Id { get; set; } = null!;
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        [MaxLength(64)]
        public string Name { get; set; } = null!;
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; } = null!;
        [BsonRepresentation(BsonType.String)]
        public string? Icon { get; set; }
        [BsonRepresentation(BsonType.String)]

        public string CreatedBy { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public GeoJsonPoint<GeoJson2DProjectedCoordinates> Location { get; set; } = null!;
        [BsonRepresentation(BsonType.Double)]
        public double Radius { get; set; } = 0;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
