using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Infrastructure.Services
{
    public class RobotService : ClientService<RobotModel, RobotResponse>, IRobotService
    {
        protected readonly IRobotRepository robotRepository;

        private async Task<UserModel> GetCurrentClient()
        {
            var client = await authService.GetUser() ?? throw new BadRequestException("Permission denied");

            switch (client.Status)
            {
                case ClientStatus.Banned:
                    throw new BadRequestException("Your account is banned");
                case ClientStatus.Inactive:
                    throw new BadRequestException("Your account is not active");
                case ClientStatus.Limited:
                    throw new BadRequestException("Your account has limited access");
                case ClientStatus.Active:
                    break;
            }
            return client;
        }

        public RobotService(IRobotRepository robotRepository, IGroupRepository groupRepository, IAuthService authService) :
            base(robotRepository, groupRepository, authService)
        {
            this.robotRepository = robotRepository;
        }

        protected override RobotResponse CreateResponse(RobotModel? entity)
        {
            return entity == null ? null : new RobotResponse(entity);
        }

        public async Task<PageResponse<RobotResponse>> Search(RobotsSearchRequest request)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Robots.CanRead);
            var robots = await robotRepository.Search(request, client.AccessLevel);
            return new PageResponse<RobotResponse>(robots.Items.Select(r => new RobotResponse(r)).ToList(), robots.Total, request);
        }

        public async Task<RobotResponse> CreateRobot(CreateRobotRequest request)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Robots.CanCreate);

            var robot = new RobotModel
            {
                Nickname = request.Nickname,
                Description = request.Description,
                Status = request.Status,
                ScanRadius = request.ScanRadius,
                UpdateLocationSecondsInterval = request.UpdateLocationSecondsInterval,
                AccessLevel = request.AccessLevel,
                GroupId = request.GroupId
            };

            robot = await robotRepository.Create(robot);
            return robot;
        }

        public async Task<RobotResponse> Get(string id)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Robots.CanRead);

            var robot = await robotRepository.Get(id) ?? throw new BadRequestException("Robot not found");
            return new RobotResponse(robot);
        }

        public async Task<RobotResponse> UpdateRobot(string id, UpdateRobotRequest request)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Robots.CanUpdate);

            var robot = await robotRepository.Get(id) ?? throw new BadRequestException("Robot not found");
            robot.Nickname = request.Nickname;
            robot.Description = request.Description;

            if(robot.GroupId != request.GroupId)
            {
                var group = await groupRepository.Get(request.GroupId) ?? throw new BadRequestException("Group not found");
                robot.GroupId = request.GroupId;
            }

            robot = await robotRepository.Update(robot);
            return new RobotResponse(robot);
        }

        public async Task<RobotTokenResponse> GetToken(string id)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Robots.CanRead);

            var robot = await robotRepository.Get(id) ?? throw new BadRequestException("Robot not found");
            return new RobotTokenResponse(robot);
        }
    }
}
