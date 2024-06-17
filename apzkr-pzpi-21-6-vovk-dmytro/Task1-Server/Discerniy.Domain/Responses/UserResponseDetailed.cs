using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Responses
{
    public class UserResponseDetailed : UserResponse
    {
        public IList<string> Groups { get; set; } = new List<string>();
        public DateTime? LastOnline { get; set; }
        public int AccessLevel { get; set; }
        public int ScanRadius { get; set; }
        public bool NeedPasswordChange { get; set; } = true;
        public string Description { get; set; }
        public GeoCoordinates? Location { get; set; }

        public UserResponseDetailed(UserModel user)
            : base(user)
        {
            Groups = user.Groups;
            LastOnline = user.LastOnline;
            AccessLevel = user.AccessLevel;
            ScanRadius = user.ScanRadius;
            Description = user.Description ?? "";
            NeedPasswordChange = user.NeedPasswordChange;
            if(user.Location != null)
            {
                Location = new GeoCoordinates(user.Location.Coordinates);
            }
        }

        public static implicit operator UserResponseDetailed(UserModel user) => new UserResponseDetailed(user);
    }
}
