using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Exceptions;
using MongoDB.Driver.GeoJsonObjectModel;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Entity.DomainEntity;

namespace Discerniy.Infrastructure.Services
{
    public abstract class ClientService<T, Response> : IClientService<T, Response> where T : class, IClient
    {
        protected readonly IRepository<T> clientRepository;
        protected readonly IGroupRepository groupRepository;
        protected readonly IAuthService authService;

        protected IGroupRepository GroupRepository => groupRepository;

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

        protected abstract Response CreateResponse(T? entity);

        public ClientService(IRepository<T> clientRepository, IGroupRepository groupRepository, IAuthService authService)
        {
            this.clientRepository = clientRepository;
            this.groupRepository = groupRepository;
            this.authService = authService;
        }

        public async Task<Response> UpdateLocation(GeoJson2DProjectedCoordinates location)
        {
            var currentClient = await authService.GetUser() ?? throw new BadRequestException("Permission denied");
            var user = await clientRepository.Get(currentClient.Id) ?? throw new BadRequestException("Client not found");
            if (user.Status == ClientStatus.Banned)
            {
                throw new BadRequestException("Your account is banned");
            }
            if (user.Status == ClientStatus.Inactive)
            {
                throw new BadRequestException("Your account is not active");
            }
            user.Location = new GeoJsonPoint<GeoJson2DProjectedCoordinates>(location);
            return CreateResponse(await clientRepository.Update(user));
        }

        public async Task<Response> UpdateStatus(string id, ClientStatus status)
        {
            var currentClient = await GetCurrentClient();
            var user = await clientRepository.Get(id) ?? throw new BadRequestException("Client not found");
            if (user.Type == ClientType.Robot)
            {
                currentClient.Permissions.Has(p => p.Robots.CanUpdateStatus);
            }
            else
            {
                currentClient.Permissions.Has(p => p.Users.CanUpdateStatus);
            }
            if (currentClient.AccessLevel <= user.AccessLevel)
            {
                throw new BadRequestException("You cannot change the status of a client with a higher access level than your own");
            }

            user.Status = status;
            return CreateResponse(await clientRepository.Update(user));
        }

        public async Task<Response> UpdateScanRadius(string id, int radius)
        {
            if (radius < 0)
            {
                throw new BadRequestException("Scan radius cannot be negative");
            }
            var currentClient = await GetCurrentClient();
            var user = await clientRepository.Get(id) ?? throw new BadRequestException("Client not found");
            if (user.Type == ClientType.Robot)
            {
                currentClient.Permissions.Has(p => p.Robots.CanUpdateScanRadius);
            }
            else
            {
                currentClient.Permissions.Has(p => p.Users.CanUpdateScanRadius);
            }
            if (currentClient.AccessLevel <= user.AccessLevel)
            {
                throw new BadRequestException("You cannot change the scan radius of a client with a higher access level than your own");
            }
            user.ScanRadius = radius;
            return CreateResponse(await clientRepository.Update(user));
        }

        public async Task<Response> UpdateAccessLevel(string id, int accessLevel)
        {
            var currentClient = await GetCurrentClient();
            var user = await clientRepository.Get(id) ?? throw new BadRequestException("User not found");
            if (user.Type == ClientType.Robot)
            {
                currentClient.Permissions.Has(p => p.Robots.CanUpdateAccessLevel);
            }
            else
            {
                currentClient.Permissions.Has(p => p.Users.CanUpdateAccessLevel);
            }
            if (user.AccessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("You cannot change the access level of a user with a higher access level than your own");
            }
            if (accessLevel < 0)
            {
                throw new BadRequestException("Access level cannot be negative");
            }
            if (accessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("Access level cannot be higher or equal to your own");
            }
            user.AccessLevel = accessLevel;
            return CreateResponse(await clientRepository.Update(user));
        }

        public async Task<Response?> Delete(string id)
        {
            var currentClient = await GetCurrentClient();
            currentClient.Permissions.Has(p => p.Users.CanDelete);
            var user = await clientRepository.Get(id) ?? throw new BadRequestException("User not found");

            if (user.Type == ClientType.Robot)
            {
                currentClient.Permissions.Has(p => p.Robots.CanDelete);
            }
            else
            {
                currentClient.Permissions.Has(p => p.Users.CanDelete);
            }

            if (user.Id == currentClient.Id)
            {
                throw new BadRequestException("You cannot delete yourself");
            }

            if (user.AccessLevel >= currentClient.AccessLevel)
            {
                throw new BadRequestException("You cannot delete a user with a higher access level than your own");
            }

            try
            {
                var groups = await GroupRepository.GetByMember(id);
                foreach (var group in groups)
                {
                    group.Members.Remove(id);
                    await GroupRepository.Update(group);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return CreateResponse(await clientRepository.Delete(id));
        }
    }
}
