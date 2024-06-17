using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Interface.Entity
{
    public interface IUpdatable
    {
        void Update(ClientPermissions newPermissions, ClientPermissions executorsPermissions);
    }
}
