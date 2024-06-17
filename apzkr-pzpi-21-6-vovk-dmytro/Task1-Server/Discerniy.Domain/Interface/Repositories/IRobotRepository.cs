using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Domain.Interface.Repositories
{
    public interface IRobotRepository : IRepository<RobotModel>
    {
        Task<PageResponse<RobotModel>> Search(ClientSearchRequest request, int accessLevel);
        Task<PageResponse<RobotModel>> Search(RobotsSearchRequest request, int accessLevel);
        Task<IList<LocationResponse>> GetNearRobots(GeoJson2DProjectedCoordinates coordinates, double radius);
    }
}
