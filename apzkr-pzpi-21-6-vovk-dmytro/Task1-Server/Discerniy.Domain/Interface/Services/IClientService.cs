using Discerniy.Domain.Interface.Entity;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Domain.Interface.Services
{
    public interface IClientService<T, Response> where T : IClient
    {
        Task<Response> UpdateLocation(GeoJson2DProjectedCoordinates location);
        Task<Response> UpdateStatus(string id, ClientStatus status);
        Task<Response> UpdateAccessLevel(string id, int accessLevel);
        Task<Response> UpdateScanRadius(string id, int radius);
        Task<Response?> Delete(string id);
    }
}
