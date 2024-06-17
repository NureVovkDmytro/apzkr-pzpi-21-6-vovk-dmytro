using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Domain.Interface.Repositories
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<PageResponse<UserModel>> GetAll(PageRequest request, int maxAccessLevel);
        Task<UserModel?> GetByEmail(string email);
        Task<PageResponse<UserModel>> Search(ClientSearchRequest request, int accessLevel);
        Task<PageResponse<UserModel>> Search(UsersSearchRequest request, int accessLevel);

        Task<ClientSession> CreateSession(string userId);
        Task<UserModel?> UpdateSession(string userId, ClientSession session);
        Task<UserModel?> RemoveSession(string userId, ClientSession session);
        Task<bool> ExistsByEmail(string email);
        Task<bool> ExistsByTaxPayerId(string taxId);

        Task<IList<LocationResponse>> GetNearUsers(GeoJson2DProjectedCoordinates coordinates, double radius);
    }
}
