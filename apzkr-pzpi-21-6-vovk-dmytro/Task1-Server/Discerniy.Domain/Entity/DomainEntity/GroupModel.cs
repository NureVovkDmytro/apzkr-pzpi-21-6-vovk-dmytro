using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Entity.DomainEntity
{
    public class GroupModel : IIdentifier
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; } = "";
        public IDictionary<string, GroupMemberModel> Members { get; set; } = new Dictionary<string, GroupMemberModel>();
        public IList<string> Admins { get; set; } = new List<string>();
        public IList<string> Marks { get; set; } = new List<string>();

        [BsonRepresentation(BsonType.Int32)]
        /// <summary>
        /// This is minimum access level required to join the group
        /// </summary>
        public int AccessLevel { get; set; } = 0;

        public GeoJsonPolygon<GeoJson2DProjectedCoordinates> ResponsibilityArea { get; set; } = null!;

        [BsonIgnore]
        public string ResponsibilityAreaCacheKey => $"group.{Id}.ResponsibilityArea";
    }
}
