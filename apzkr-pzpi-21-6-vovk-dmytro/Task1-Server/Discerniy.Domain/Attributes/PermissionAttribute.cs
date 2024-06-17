namespace Discerniy.Domain.Attributes
{
    public class PermissionAttribute : Attribute
    {
        public string Name { get; }

        public PermissionAttribute(string permissionName)
        {
            Name = permissionName;
        }
    }
}
