using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Infrastructure.Repository
{
    public class ClientRepository : IClientRepository
    {
        protected readonly IUserRepository userRepository;
        protected readonly IRobotRepository robotRepository;

        public ClientRepository(IUserRepository userRepository, IRobotRepository robotRepository)
        {
            this.userRepository = userRepository;
            this.robotRepository = robotRepository;
        }

        public ClientType GetClientType(object entity)
        {
            var user = entity as UserModel;
            if (user != null && user.Id.StartsWith("U"))
            {
                return ClientType.User;
            }
            var robot = entity as RobotModel;
            if (robot != null && robot.Id.StartsWith("R"))
            {
                return ClientType.Robot;
            }
            throw new NotImplementedException();
        }

        public ClientType GetClientTypeById(string id)
        {
            if(id.StartsWith("U"))
            {
                return ClientType.User;
            }
            if(id.StartsWith("R"))
            {
                return ClientType.Robot;
            }
            throw new NotImplementedException();
        }

        public async Task<IClient> Create(object entity)
        {
            switch (GetClientType(entity))
            {
                case ClientType.User:
                    return await userRepository.Create(entity as UserModel);
                case ClientType.Robot:
                    return await robotRepository.Create(entity as RobotModel);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<IClient?> Get(string id)
        {
            switch (GetClientTypeById(id))
            {
                case ClientType.User:
                    return await userRepository.Get(id);
                case ClientType.Robot:
                    return await robotRepository.Get(id);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<IClient> Update(object entity)
        {
            switch (GetClientType(entity))
            {
                case ClientType.User:
                    return await userRepository.Update(entity as UserModel);
                case ClientType.Robot:
                    return await robotRepository.Update(entity as RobotModel);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<IClient?> Delete(string id)
        {
            switch (GetClientTypeById(id))
            {
                case ClientType.User:
                    return await userRepository.Delete(id);
                case ClientType.Robot:
                    return await robotRepository.Delete(id);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<bool> Exists(string id)
        {
            switch (GetClientTypeById(id))
            {
                case ClientType.User:
                    return await userRepository.Exists(id);
                case ClientType.Robot:
                    return await robotRepository.Exists(id);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<PageResponse<IClient>> Search(ClientSearchRequest request, int minAccessLevel)
        {
            var users = userRepository.Search(request, minAccessLevel);
            var robots = robotRepository.Search(request, minAccessLevel);
            await Task.WhenAll(users, robots);
            var items = users.Result.Items.Select(x => x as IClient)
                .Concat(robots.Result.Items.Select(x => x as IClient))
                .ToList();
            return new PageResponse<IClient>(items, users.Result.Total + robots.Result.Total, request);
        }

        public async Task<IList<LocationResponse>> GetNearClients(GeoJson2DProjectedCoordinates coordinates, double radius)
        {
            List<LocationResponse> clients = new List<LocationResponse>();
            var users = await userRepository.GetNearUsers(coordinates, radius);
            var robots = await robotRepository.GetNearRobots(coordinates, radius);
            clients.AddRange(users);
            clients.AddRange(robots);
            return clients;
        }
    }
}
