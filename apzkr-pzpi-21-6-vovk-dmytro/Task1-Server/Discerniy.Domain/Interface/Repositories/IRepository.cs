using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Repositories
{
    public interface IRepository<T> where T : class, IIdentifier
    {
        Task<T> Create(T entity);
        Task<T?> Get(string id);
        Task<PageResponse<T>> GetAll(PageRequest request);
        Task<T> Update(T entity);
        Task<T?> Delete(string id);

        Task<long> GetCountAsync();
        Task<bool> Exists(string id);
    }
}
