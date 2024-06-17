
using Discerniy.Domain.Attributes;

namespace Discerniy.Domain.Entity.SubEntity
{
    [Permission("Groups")]
    public class GroupsInteractionPermissions : BaseInteractionPermissions
    {
        public static GroupsInteractionPermissions Admin => new GroupsInteractionPermissions
        {
            CanCreate = true,
            CanRead = true,
            CanUpdate = true,
            CanDelete = true,
            CanManageMarks = true
        };

        [Permission("CanCreate")]
        public bool CanCreate { get; set; }
        [Permission("CanRead")]
        public bool CanRead { get; set; }
        [Permission("CanUpdate")]
        public bool CanUpdate { get; set; }
        [Permission("CanDelete")]
        public bool CanDelete { get; set; }
        [Permission("CanManageMarks")]
        public bool CanManageMarks { get; set; }
    }
}
