using Discerniy.Domain.Entity.SubEntity;

namespace Discerniy.Domain.Interface.Entity
{
    public interface IPermissionQuery
    {
        IPermissionQuery Has(Func<ClientPermissions, bool> parameter, string? errorMessage = null);
    }
}
