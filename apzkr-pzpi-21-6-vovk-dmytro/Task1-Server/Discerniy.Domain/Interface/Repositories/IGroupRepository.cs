using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Repositories
{
    public interface IGroupRepository : IRepository<GroupModel>
    {
        Task<IList<GroupModel>> GetByMember(string userId);
        Task<PageResponse<GroupModel>> Search(GroupSearchRequest request);

        Task<bool> IsInsideOnGroupArea(UserModel client);
        Task<bool> IsInsideOnGroupArea(UserModel client, string groupId);
    }
}
