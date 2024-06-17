using Discerniy.Domain.Attributes;

namespace Discerniy.Domain.Entity.SubEntity
{
    [Permission("Users")]
    public class UsersInteractionPermissions : BaseInteractionPermissions
    {
        public static UsersInteractionPermissions Admin => new UsersInteractionPermissions
        {
            CanCreate = true,
            CanDelete = true,
            CanRead = true,
            CanUpdateSelfEmail = true,
            CanUpdateBaseInformation = true,
            CanResetPassword = true,
            CanUpdateStatus = true,
            CanUpdateAccessLevel = true,
            CanUpdateScanRadius = true,
            CanUpdatePermissions = true,
            CanCreateDeviceToken = true
        };

        [Permission("CanDelete")]
        public bool CanDelete { get; set; }
        [Permission("CanUpdateSelfEmail")]
        public bool CanUpdateSelfEmail { get; set; }
        [Permission("CanUpdateBaseInformation")]
        public bool CanUpdateBaseInformation { get; set; }
        [Permission("CanResetPassword")]
        public bool CanResetPassword { get; set; }
        [Permission("CanUpdateStatus")]
        public bool CanUpdateStatus { get; set; }
        [Permission("CanUpdateAccessLevel")]
        public bool CanUpdateAccessLevel { get; set; }
        [Permission("CanUpdateScanRadius")]
        public bool CanUpdateScanRadius { get; set; }
        [Permission("CanUpdatePermissions")]
        public bool CanUpdatePermissions { get; set; }
        [Permission("CanRead")]
        public bool CanRead { get; set; }
        [Permission("CanCreate")]
        public bool CanCreate { get; set; }
        [Permission("CanCreateDeviceToken")]
        public bool CanCreateDeviceToken { get; set; }
    }
}
