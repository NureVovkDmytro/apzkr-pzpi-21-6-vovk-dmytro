using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Services
{
    public interface IGroupService
    {
        Task<GroupResponse> Create(CreateGroupRequest request);
        Task<GroupResponse?> Get(string id);
        Task<GroupDetailResponse?> GetDetail(string id);
        Task<IList<GroupResponse>> GetMyGroups();
        Task<IList<GroupResponse>> GetGroupsByMember(string userId);
        Task<PageResponse<UserResponse>> GetMembers(string groupId, SearchGroupMemberRequest request);
        Task<PageResponse<GroupResponse>> Search(GroupSearchRequest request);
        Task<GroupResponse> Delete(string id);
        Task<GroupResponse> AddMember(string groupId, string userId);
        Task<GroupResponse> RemoveMember(string groupId, string userId);
        Task<GroupResponse> AddAdmin(string groupId, string userId);
        Task<GroupResponse> RemoveAdmin(string groupId, string userId);
        Task<GroupResponse> Update(string groupId, UpdateGroupRequest request);
        Task<GroupResponse> UpdateArea(string groupId, UpdateGroupAreaRequest request);
    }
}
