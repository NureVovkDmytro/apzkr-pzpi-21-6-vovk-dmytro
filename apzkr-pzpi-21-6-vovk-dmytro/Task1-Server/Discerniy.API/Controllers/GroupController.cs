using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Route("api/group")]
    [Authorize(Roles = "user")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupService groupService;

        public GroupController(IGroupService groupService)
        {
            this.groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> Searsh([FromQuery] GroupSearchRequest request)
        {
            return await RunService(() => groupService.Search(request));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyGroups()
        {
            return await RunService(() => groupService.GetMyGroups());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return await RunService(() => groupService.Get(id));
        }
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetDetail(string id)
        {
            return await RunService(() => groupService.GetDetail(id));
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembers(string id, [FromQuery] SearchGroupMemberRequest request)
        {
            return await RunService(() => groupService.GetMembers(id, request));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
        {
            return await RunService(() => groupService.Create(request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return await RunService(() => groupService.Delete(id));
        }


        [HttpPost("{groupId}/members/{userId}")]
        public async Task<IActionResult> AddMember(string groupId, string userId)
        {
            return await RunService(() => groupService.AddMember(groupId, userId));
        }

        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(string groupId, string userId)
        {
            return await RunService(() => groupService.RemoveMember(groupId, userId));
        }

        [HttpPost("{groupId}/admin/{userId}")]
        public async Task<IActionResult> AddAdmin(string groupId, string userId)
        {
            return await RunService(() => groupService.AddAdmin(groupId, userId));
        }

        [HttpDelete("{groupId}/admin/{userId}")]
        public async Task<IActionResult> RemoveAdmin(string groupId, string userId)
        {
            return await RunService(() => groupService.RemoveAdmin(groupId, userId));
        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> SetAccessLevel(string groupId, [FromBody] UpdateGroupRequest request)
        {
            return await RunService(() => groupService.Update(groupId, request));
        }
        [HttpPut("{groupId}/area")]
        public async Task<IActionResult> SetArea(string groupId, [FromBody] UpdateGroupAreaRequest request)
        {
            return await RunService(() => groupService.UpdateArea(groupId, request));
        }
    }
}
