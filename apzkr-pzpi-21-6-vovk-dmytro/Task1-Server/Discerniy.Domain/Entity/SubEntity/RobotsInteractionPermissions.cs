using Discerniy.Domain.Attributes;

namespace Discerniy.Domain.Entity.SubEntity
{
    [Permission("Robots")]
    public class RobotsInteractionPermissions : BaseInteractionPermissions
    {
        public static RobotsInteractionPermissions Admin => new RobotsInteractionPermissions
        {
            CanCreate = true,
            CanDelete = true,
            CanRead = true,
            CanUpdate = true,
            CanUpdateStatus = true,
            CanUpdateScanRadius = true,
            CanUpdateAccessLevel = true
        };
        [Permission("CanRead")]
        public bool CanRead { get; set; }
        [Permission("CanCreate")]
        public bool CanCreate { get; set; }
        [Permission("CanUpdate")]
        public bool CanUpdate { get; set; }
        [Permission("CanUpdateStatus")]
        public bool CanUpdateStatus { get; set; }
        [Permission("CanUpdateScanRadius")]
        public bool CanUpdateScanRadius { get; set; }
        [Permission("CanUpdateAccessLevel")]
        public bool CanUpdateAccessLevel { get; set; }
        [Permission("CanDelete")]
        public bool CanDelete { get; set; }
    }
}
