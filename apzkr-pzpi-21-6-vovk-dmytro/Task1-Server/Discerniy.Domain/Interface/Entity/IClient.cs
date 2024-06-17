using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using System.ComponentModel.DataAnnotations;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Interface.Entity
{
    public enum ClientType
    {
        User = 0x0,
        Robot = 0x2,
    }
    public enum ClientStatus
    {
        Inactive = 0x0, // Can`t do anything
        Active = 0x1, // Can do anything according to their level of access
        Limited = 0x2, // Only can be sent own location
        Banned = 0x3 // Can`t do anything
    }
    public interface IClient : IIdentifier
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        [MaxLength(256)]
        [BsonRepresentation(BsonType.String)]
        public string? Description { get; set; }
        [BsonRepresentation(BsonType.String)]
        [MaxLength(64)]
        public string Nickname { get; set; }
        public DateTime CreatedAt { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? LastOnline { get; set; }
        [BsonRepresentation(BsonType.Boolean)]
        public bool TokenAutoRefresh { get; set; }
        public GeoJsonPoint<GeoJson2DProjectedCoordinates>? Location { get; set; }
        public double Compass { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public ClientType Type { get; }
        [BsonRepresentation(BsonType.Int32)]
        public ClientStatus Status { get; set; }
        [BsonRepresentation(BsonType.Int32)]
        /// <summary>
        /// In meters. Client can be found by other clients in this radius.
        /// </summary>
        public int ScanRadius { get; set; }
        public int UpdateLocationSecondsInterval { get; set; }
        public IList<string> WebSocketConections { get; set; }
        /// <summary>
        /// Client access level. Clients can operate on other clients that have a <b>lower</b> access level.
        /// </summary>
        [BsonRepresentation(BsonType.Int32)]
        public int AccessLevel { get; set; }
    }
}
