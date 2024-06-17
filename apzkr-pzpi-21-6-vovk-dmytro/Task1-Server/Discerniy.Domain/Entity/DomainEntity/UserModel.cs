using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDB.Driver.GeoJsonObjectModel;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Entity.DomainEntity
{
    public class UserModel : IClient
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null!;
        [Required]
        [BsonRepresentation(BsonType.String)]
        public string FirstName { get; set; } = null!;
        [Required]
        [BsonRepresentation(BsonType.String)]
        public string LastName { get; set; } = null!;
        [Required]
        [BsonRepresentation(BsonType.String)]
        public string TaxPayerId { get; set; } = null!;
        [BsonRepresentation(BsonType.String)]
        public string Nickname { get; set; } = "";
        [MaxLength(256)]
        [BsonRepresentation(BsonType.String)]
        public string? Description { get; set; }
        [Required]
        [EmailAddress]
        [BsonRepresentation(BsonType.String)]
        public string Email { get; set; } = null!;
        [Required]
        [BsonRepresentation(BsonType.String)]
        public string Password { get; set; } = null!;
        [BsonRepresentation(BsonType.Boolean)]
        public bool NeedPasswordChange { get; set; } = true;
        [Required]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? LastOnline { get; set; } = null;
        [BsonRepresentation(BsonType.Boolean)]
        public bool TokenAutoRefresh { get; set; }
        public ClientType Type => ClientType.User;
        public GeoJsonPoint<GeoJson2DProjectedCoordinates> Location { get; set; }
        [BsonRepresentation(BsonType.Double)]
        public double Compass { get; set; }
        public IList<ClientSession> Sessions { get; set; } = new List<ClientSession>();
        public string SecretKey { get; set; } = null!;

        public ClientPermissions Permissions { get; set; } = new ClientPermissions();

        public ClientStatus Status { get; set; } = ClientStatus.Inactive;
        public IList<string> Groups { get; set; } = new List<string>();
        public int AccessLevel { get; set; } = 0;
        public int ScanRadius { get; set; } = 10;
        public int UpdateLocationSecondsInterval { get; set; } = 10;
        public IList<string> WebSocketConections { get; set; } = new List<string>();
    }
}
