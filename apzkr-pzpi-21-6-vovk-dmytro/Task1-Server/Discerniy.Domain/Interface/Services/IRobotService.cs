using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Services
{
    public interface IRobotService : IClientService<RobotModel, RobotResponse>
    {
        Task<PageResponse<RobotResponse>> Search(RobotsSearchRequest request);
        Task<RobotTokenResponse> GetToken(string id);
        Task<RobotResponse> CreateRobot(CreateRobotRequest request);
        Task<RobotResponse> Get(string id);
        Task<RobotResponse> UpdateRobot(string id, UpdateRobotRequest request);
    }
}
