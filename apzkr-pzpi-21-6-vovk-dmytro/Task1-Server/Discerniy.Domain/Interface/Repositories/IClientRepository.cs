using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Domain.Interface.Repositories
{
    public interface IClientRepository
    {
        Task<IClient> Create(object entity);
        Task<IClient?> Get(string id);
        Task<IClient> Update(object entity);
        Task<IClient?> Delete(string id);
        Task<PageResponse<IClient>> Search(ClientSearchRequest request, int minAccessLevel);
        Task<IList<LocationResponse>> GetNearClients(GeoJson2DProjectedCoordinates coordinates, double radius);
        Task<bool> Exists(string id);
    }
}
