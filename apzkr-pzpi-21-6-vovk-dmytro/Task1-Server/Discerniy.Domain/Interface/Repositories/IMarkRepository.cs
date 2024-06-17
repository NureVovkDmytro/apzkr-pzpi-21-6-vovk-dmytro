using Discerniy.Domain.Entity.DomainEntity;

namespace Discerniy.Domain.Interface.Repositories
{
    public interface IMarkRepository : IRepository<MarkModel>
    {
        Task<IEnumerable<MarkModel>> GetFromGroup(string groupId);
    }
}
