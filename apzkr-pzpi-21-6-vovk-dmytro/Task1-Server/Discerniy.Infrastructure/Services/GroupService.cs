using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Infrastructure.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly IUserRepository userRepository;
        private readonly IAuthService authService;
        private readonly IDistributedCache cache;

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

        public GroupService(IGroupRepository groupRepository, IUserRepository userRepository, IAuthService authService, IDistributedCache cache)
        {
            this.groupRepository = groupRepository;
            this.userRepository = userRepository;
            this.authService = authService;
            this.cache = cache;
        }

        public async Task<GroupResponse> AddAdmin(string groupId, string userId)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions
                .Has(p => p.Groups.CanUpdate).Has(p => p.Groups.CanRead)
                .Has(p => p.Users.CanRead);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Admins.Contains(currentUser.Id))
            {
                throw new BadRequestException("Permission denied");
            }
            var user = await userRepository.Get(userId) ?? throw new BadRequestException("User not found");

            if (!group.Members.ContainsKey(userId))
            {
                throw new BadRequestException("User is not a member");
            }
            if (group.Admins.Contains(userId))
            {
                throw new BadRequestException("User is already an admin");
            }
            group.Admins.Add(userId);
            var updatedGroup = await groupRepository.Update(group);
            return updatedGroup;
        }

        public async Task<GroupResponse> AddMember(string groupId, string userId)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions
                .Has(p => p.Groups.CanUpdate).Has(p => p.Groups.CanRead)
                .Has(p => p.Users.CanRead);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Admins.Contains(currentUser.Id))
            {
                throw new BadRequestException("Permission denied");
            }
            var user = await userRepository.Get(userId) ?? throw new BadRequestException("User not found");

            if (user.AccessLevel < group.AccessLevel)
            {
                throw new BadRequestException("User has insufficient access level");
            }
            if (group.Members.ContainsKey(userId))
            {
                throw new BadRequestException("User is already a member");
            }
            group.Members.Add(userId, new GroupMemberModel(user));
            user.Groups.Add(groupId);
            var updatedGroup = await groupRepository.Update(group);
            var updatedUser = await userRepository.Update(user);
            return updatedGroup;
        }

        public async Task<GroupResponse> Create(CreateGroupRequest request)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanCreate);

            if(currentUser.AccessLevel <= request.AccessLevel)
            {
                throw new BadRequestException("Access level is too high");
            }

            var group = new GroupModel()
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                AccessLevel = request.AccessLevel,
            };
            group.CreatedAt = DateTime.UtcNow;
            group.Members.Add(currentUser.Id, new GroupMemberModel(currentUser));
            group.Admins.Add(currentUser.Id);
            var createdGroups = await groupRepository.Create(group);
            currentUser.Groups.Add(createdGroups.Id);
            var updatedUser = await userRepository.Update(currentUser);
            return createdGroups;
        }

        public async Task<GroupResponse> Delete(string id)
        {
            var currentUser = await authService.GetUser();
            currentUser.Permissions.Has(p => p.Groups.CanDelete);

            var group = await groupRepository.Get(id);
            if (group == null)
            {
                throw new BadRequestException("Group not found");
            }
            if (!group.Admins.Contains(currentUser.Id))
            {
                throw new BadRequestException("Permission denied");
            }
            // delete group from all members
            foreach (var memberId in group.Members)
            {
                var member = await userRepository.Get(memberId.Key);
                if (member == null)
                {
                    continue;
                }
                member.Groups.Remove(id);
                await userRepository.Update(member);
            }
            return await groupRepository.Delete(id);
        }

        public async Task<GroupResponse?> Get(string id)
        {
            return await GetDetail(id);
        }

        public async Task<GroupDetailResponse?> GetDetail(string id)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanRead);

            if (!currentUser.Groups.Contains(id))
            {
                throw new BadRequestException("Permission denied");
            }
            var group = await groupRepository.Get(id) ?? throw new BadRequestException("Group not found");

            if (group.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("Permission denied");
            }
            return group;
        }

        public async Task<IList<GroupResponse>> GetGroupsByMember(string userId)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanRead).Has(p => p.Users.CanRead);

            var user = await userRepository.Get(userId) ?? throw new BadRequestException("User not found");

            if (user.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("Permission denied");
            }
            var groups = new List<GroupModel>();
            bool hasListChanged = false;
            foreach (var groupId in user.Groups)
            {
                var group = await groupRepository.Get(groupId);
                if (group == null)
                {
                    user.Groups.Remove(groupId);
                    hasListChanged = true;
                    continue;
                }
                if (group.AccessLevel > user.AccessLevel)
                {
                    user.Groups.Remove(groupId);
                    hasListChanged = true;
                    continue;
                }
                groups.Add(group);
            }
            if (hasListChanged)
            {
                await userRepository.Update(user);
            }
            return groups.Select(g => new GroupResponse(g)).ToList();
        }

        public async Task<IList<GroupResponse>> GetMyGroups()
        {
            var currentUser = await GetCurrentClient();

            var groups = new List<GroupModel>();
            bool hasListChanged = false;

            for (int i = 0; i < currentUser.Groups.Count; i++)
            {
                var groupId = currentUser.Groups[i];
                var group = await groupRepository.Get(groupId);
                if (group == null)
                {
                    hasListChanged = true;
                    currentUser.Groups.Remove(groupId);
                    continue;
                }
                if (group.AccessLevel > currentUser.AccessLevel)
                {
                    hasListChanged = true;
                    currentUser.Groups.Remove(groupId);
                    continue;
                }
                groups.Add(group);
            }
            if (hasListChanged)
            {
                await userRepository.Update(currentUser);
            }
            return groups.Select(g => new GroupResponse(g)).ToList();
        }

        public async Task<PageResponse<GroupResponse>> Search(GroupSearchRequest request)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanRead);

            var groups = await groupRepository.Search(request);

            return new PageResponse<GroupResponse>(groups.Items.Select(p => new GroupResponse(p)).ToList(), groups.Total, request);
        }

        public async Task<GroupResponse> RemoveAdmin(string groupId, string userId)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions
                .Has(p => p.Groups.CanUpdate).Has(p => p.Groups.CanRead)
                .Has(p => p.Users.CanRead);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Admins.Contains(currentUser.Id))
            {
                throw new BadRequestException("Permission denied");
            }
            if (group.Admins.Count == 1)
            {
                throw new BadRequestException("Group must have at least one admin");
            }
            if (!group.Admins.Contains(userId))
            {
                throw new BadRequestException("User is not an admin");
            }
            var user = await userRepository.Get(userId);
            if (user == null)
            {
                group.Members.Remove(userId);
                group.Admins.Remove(userId);
                return await groupRepository.Update(group);
            }
            if (user.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("User has higher access level");
            }
            group.Admins.Remove(userId);
            var updatedGroup = await groupRepository.Update(group);
            return updatedGroup;
        }

        public async Task<GroupResponse> RemoveMember(string groupId, string userId)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions
                .Has(p => p.Groups.CanUpdate).Has(p => p.Groups.CanRead)
                .Has(p => p.Users.CanRead);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");
 
            if (!group.Admins.Contains(currentUser.Id))
            {
                throw new BadRequestException("Permission denied");
            }
            if (!group.Members.ContainsKey(userId))
            {
                throw new BadRequestException("User is not a member");
            }
            var user = await userRepository.Get(userId);
            if (user == null)
            {
                group.Members.Remove(userId);
                group.Admins.Remove(userId);
                return await groupRepository.Update(group);
            }
            if (user.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("User has higher access level");
            }
            group.Members.Remove(userId);
            group.Admins.Remove(userId);
            user.Groups.Remove(groupId);
            var updatedGroup = await groupRepository.Update(group);
            var updatedUser = await userRepository.Update(user);
            if (updatedGroup.Members.Count == 0)
            {
                return await groupRepository.Delete(groupId);
            }
            return updatedGroup;
        }

        public async Task<GroupResponse> Update(string groupId, UpdateGroupRequest request)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanUpdate);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Admins.Contains(currentUser.Id) || group.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("Permission denied");
            }

            if(request.AccessLevel >= currentUser.AccessLevel)
            {
                throw new BadRequestException("Access level is too high");
            }

            group.Name = request.Name;
            group.Description = request.Description ?? string.Empty;
            group.AccessLevel = request.AccessLevel;
            return await groupRepository.Update(group);
        }

        public async Task<PageResponse<UserResponse>> GetMembers(string groupId, SearchGroupMemberRequest request)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanRead).Has(p => p.Users.CanRead);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Members.ContainsKey(currentUser.Id))
            {
                throw new BadRequestException("Permission denied");
            }

            if(group.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("Permission denied");
            }

            if(request.SearchType == ClientType.User)
            {
                var userMembers = group.Members.Where(m => m.Value.Type == ClientType.User).ToList();
                
                var users = await userRepository.Search(request, currentUser.AccessLevel);
                return new PageResponse<UserResponse>(users.Items.Select(u => new UserResponse(u)).ToList(), users.Total, request);
            }
            request.GroupId = groupId;
            var result = await userRepository.Search(request, currentUser.AccessLevel);
            return new PageResponse<UserResponse>(result.Items.Select(u => new UserResponse(u)).ToList(), result.Total, request);
        }

        public async Task<GroupResponse> UpdateArea(string groupId, UpdateGroupAreaRequest request)
        {
            var currentUser = await GetCurrentClient();
            currentUser.Permissions.Has(p => p.Groups.CanUpdate);

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Admins.Contains(currentUser.Id) || group.AccessLevel > currentUser.AccessLevel)
            {
                throw new BadRequestException("Permission denied");
            }

            if (request.Coordinates.First() != request.Coordinates.Last())
            {
                request.Coordinates.Add(request.Coordinates.First());
            }

            if (request.Coordinates.Count() < 4)
            {
                throw new BadRequestException("Polygon must have at least 3 coordinates");
            }


            var linearRingCoordinates = new GeoJsonLinearRingCoordinates<GeoJson2DProjectedCoordinates>(request.Coordinates.AsEnumerable());
            var polygonCoordinates = new GeoJsonPolygonCoordinates<GeoJson2DProjectedCoordinates>(linearRingCoordinates);

            group.ResponsibilityArea = new GeoJsonPolygon<GeoJson2DProjectedCoordinates>(polygonCoordinates);

            await this.cache.SetStringAsync(group.ResponsibilityAreaCacheKey, group.ResponsibilityArea.ToJson());

            return await groupRepository.Update(group);
        }
    }
}
