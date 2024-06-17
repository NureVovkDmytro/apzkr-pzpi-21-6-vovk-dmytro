using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Exceptions;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Discerniy.Infrastructure.Services
{
    public class MarkService : IMarkService
    {
        protected readonly IMarkRepository markRepository;
        protected readonly IGroupRepository groupRepository;
        protected readonly IAuthService authService;
        protected readonly IWebSocketMessagePublisher webSocketMessagePublisher;

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

        public MarkService(IMarkRepository markRepository, IGroupRepository groupRepository, IAuthService authService, IWebSocketMessagePublisher webSocketMessagePublisher)
        {
            this.markRepository = markRepository;
            this.groupRepository = groupRepository;
            this.authService = authService;
            this.webSocketMessagePublisher = webSocketMessagePublisher;
        }

        public async Task<MarkResponse> CreateMark(CreateMarkRequest request)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Groups.CanManageMarks);

            var group = await groupRepository.Get(request.GroupId) ?? throw new BadRequestException("Group not found");

            if (!group.Members.ContainsKey(client.Id))
            {
                throw new BadRequestException("You are not a member of this group");
            }

            if(await groupRepository.IsInsideOnGroupArea(client, request.GroupId) == false)
            {
                throw new BadRequestException("You are not inside the group area");
            }

            var mark = new MarkModel
            {
                GroupId = request.GroupId,
                Name = request.Name,
                Description = request.Description,
                CreatedBy = client.Id,
                Radius = request.Radius,
                Icon = request.Icon,
                Location = new GeoJsonPoint<GeoJson2DProjectedCoordinates>(request.Location),
                CreatedAt = DateTime.UtcNow
            };

            var marker = await markRepository.Create(mark);
            group.Marks.Add(marker.Id);
            await groupRepository.Update(group);

            return marker;
        }

        public async Task<MarkResponse> DeleteMark(string markId)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Groups.CanManageMarks);

            var mark = await markRepository.Get(markId) ?? throw new BadRequestException("Mark not found");

            var group = await groupRepository.Get(mark.GroupId) ?? throw new BadRequestException("Group not found");

            if (!group.Members.ContainsKey(client.Id))
            {
                throw new BadRequestException("You are not a member of this group");
            }

            if (mark.CreatedBy != client.Id)
            {
                throw new BadRequestException("You are not the creator of this mark");
            }

            await markRepository.Delete(markId);

            return mark;
        }

        public async Task<MarkResponse> GetMark(string markId)
        {
            var client = await GetCurrentClient();

            var mark = await markRepository.Get(markId) ?? throw new BadRequestException("Mark not found");

            var group = await groupRepository.Get(mark.GroupId) ?? throw new BadRequestException("Group not found");

            if (!group.Members.ContainsKey(client.Id))
            {
                throw new BadRequestException("You are not a member of this group");
            }

            return mark;
        }

        public async Task<IEnumerable<MarkResponse>> GetMarksFromGroup(string groupId)
        {
            var client = await GetCurrentClient();

            var group = await groupRepository.Get(groupId) ?? throw new BadRequestException("Group not found");

            if (!group.Members.ContainsKey(client.Id))
            {
                throw new BadRequestException("You are not a member of this group");
            }

            var marks = await markRepository.GetFromGroup(groupId);

            return marks.Select(m => new MarkResponse(m));
        }

        public async Task<MarkResponse> UpdateMark(string markId, UpdateMarkRequest request)
        {
            var client = await GetCurrentClient();
            client.Permissions.Has(p => p.Groups.CanManageMarks);

            var mark = await markRepository.Get(markId) ?? throw new BadRequestException("Mark not found");

            var group = await groupRepository.Get(mark.GroupId) ?? throw new BadRequestException("Group not found");

            if (!group.Members.ContainsKey(client.Id))
            {
                throw new BadRequestException("You are not a member of this group");
            }

            if (mark.CreatedBy != client.Id)
            {
                throw new BadRequestException("You are not the creator of this mark");
            }

            mark.Name = request.Name ?? mark.Name;
            mark.Description = request.Description ?? mark.Description;
            mark.Radius = request.Radius;
            mark.Icon = request.Icon;
            mark.Location = new GeoJsonPoint<GeoJson2DProjectedCoordinates>(request.Location);

            await markRepository.Update(mark);

            return mark;
        }
    }
}
