using Discerniy.Domain.Attributes;
using Discerniy.Domain.Interface.Entity;

namespace Discerniy.Domain.Entity.SubEntity
{
    public class ClientPermissions : IPermissionQuery
    {
        public static ClientPermissions Default => new ClientPermissions();
        public static ClientPermissions Admin => new ClientPermissions
        {
            Users = UsersInteractionPermissions.Admin,
            Robots = RobotsInteractionPermissions.Admin,
            Groups = GroupsInteractionPermissions.Admin
        };

        [Permission("Users")]
        public UsersInteractionPermissions Users { get; set; } = new UsersInteractionPermissions();
        [Permission("Robots")]
        public RobotsInteractionPermissions Robots { get; set; } = new RobotsInteractionPermissions();
        [Permission("Groups")]
        public GroupsInteractionPermissions Groups { get; set; } = new GroupsInteractionPermissions();

        /// <summary>
        /// Check if the client has the permission. If not, throw an exception with the specified message.
        /// </summary>
        /// <param name="parameter">Validation function</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Itself</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public IPermissionQuery Has(Func<ClientPermissions, bool> parameter, string? errorMessage = null)
        {
            if(parameter(this))
            {
                return this;
            }
            throw new UnauthorizedAccessException(errorMessage ?? "Permission denied");
        }
        /// <summary>
        /// Function for updating permissions. If the executor has not enough permissions, the function will throw an <see cref="BadHttpRequestException"/> exception.
        /// </summary>
        /// <param name="newPermissions"></param>
        /// <param name="executorsPermissions"></param>
        public void Update(ClientPermissions newPermissions, ClientPermissions executorsPermissions)
        {
            if(!executorsPermissions.Users.CanUpdatePermissions)
            {
                return;
            }
            Users.Update(newPermissions, executorsPermissions);
            Robots.Update(newPermissions, executorsPermissions);
            Groups.Update(newPermissions, executorsPermissions);
        }
    }
}
