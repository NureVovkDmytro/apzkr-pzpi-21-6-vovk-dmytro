using Discerniy.Domain.Attributes;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
using System.Reflection;

namespace Discerniy.Domain.Entity.SubEntity
{
    public abstract class BaseInteractionPermissions : IUpdatable
    {
        protected string PermissionName
        {
            get
            {
                var permissionAttribute = GetType().GetCustomAttribute<PermissionAttribute>();
                if (permissionAttribute == null)
                {
                    throw new BadRequestException("Permission attribute is not set.");
                }
                return permissionAttribute.Name;
            }
        }

        public virtual void Update(ClientPermissions newPermissions, ClientPermissions executorsPermissions)
        {
            var properties = GetType().GetProperties().ToList();

            foreach (var property in properties)
            {
                var permissionAttribute = property.GetCustomAttribute<PermissionAttribute>();
                if (permissionAttribute == null)
                {
                    continue;
                }
                UpdatePermissionSetting(
                    setPermission: this.SetPermission,
                    getPermission: this.GetPermission,
                    permissionName: permissionAttribute.Name,
                    newPermission: newPermissions,
                    executorPermissions: executorsPermissions
                    );
            }
        }

        protected void UpdatePermissionSetting(Action<bool, string> setPermission, Func<ClientPermissions, string, bool> getPermission, string permissionName, ClientPermissions newPermission, ClientPermissions executorPermissions)
        {
            bool newPermissionValue = getPermission(newPermission, permissionName);

            // get the current value of the permission from the object
            var property = GetType().GetProperty(permissionName);
            if (property == null)
            {
                throw new BadRequestException($"Permission '{permissionName}' does not exist.");
            }

            bool currentPermissionValue = (bool)property.GetValue(this);


            if (currentPermissionValue == newPermissionValue)
            {
                return;
            }

            if (!getPermission(executorPermissions, permissionName))
            {
                throw new BadRequestException($"You do not have permission to modify '{permissionName}'.");
            }

            setPermission(newPermissionValue, permissionName);
        }

        protected bool GetPermission(ClientPermissions permissions, string permissionName)
        {
            var permissionPropert = permissions.GetType().GetProperty(PermissionName);

            if (permissionPropert == null)
            {
                throw new BadRequestException($"Permission '{PermissionName}' does not exist.");
            }

            var permission = permissionPropert.GetValue(permissions);
            if (permission == null)
            {
                throw new BadRequestException($"Permission '{PermissionName}' does not exist.");
            }

            permissionPropert = permission.GetType().GetProperty(permissionName);
            if (permissionPropert == null)
            {
                throw new BadRequestException($"Permission '{permissionName}' does not exist.");
            }

            var permissionValue = permissionPropert.GetValue(permission);
            if (permissionValue == null)
            {
                throw new BadRequestException($"Permission '{permissionName}' does not exist.");
            }

            if (!(permissionValue is bool))
            {
                throw new BadRequestException($"Permission '{permissionName}' is not a boolean.");
            }

            return (bool)permissionValue;
        }

        protected void SetPermission(bool value, string permissionName)
        {
            var property = GetType().GetProperty(permissionName);
            if (property == null)
            {
                throw new BadRequestException($"Permission '{permissionName}' does not exist.");
            }

            property.SetValue(this, value);
        }

    }
}
