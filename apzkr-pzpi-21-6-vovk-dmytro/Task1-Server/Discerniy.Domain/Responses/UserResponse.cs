using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string TaxPayerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClientType Type => ClientType.User;
        public ClientPermissions Permissions { get; set; } = new ClientPermissions();
        public int UpdateLocationSecondsInterval { get; set; }
        public ClientStatus Status { get; set; } = ClientStatus.Inactive;
        public DateTime? LastOnline { get; set; }
        public int AccessLevel { get; set; } 

        public UserResponse(UserModel user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Nickname = user.Nickname;
            Email = user.Email;
            TaxPayerId = user.TaxPayerId;
            CreatedAt = user.CreatedAt;
            Permissions = user.Permissions;
            UpdateLocationSecondsInterval = user.UpdateLocationSecondsInterval;
            Status = user.Status;
            LastOnline = user.LastOnline;
            AccessLevel = user.AccessLevel;
        }

        public static implicit operator UserResponse(UserModel user) => new UserResponse(user);
    }
}
