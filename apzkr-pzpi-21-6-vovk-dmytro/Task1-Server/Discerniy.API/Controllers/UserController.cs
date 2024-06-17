using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Authorize(Roles = "user")]
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersSearchRequest request)
        {
            return await RunService(() => userService.Search(request));
        }

        [HttpGet("self")]
        public async Task<IActionResult> GetSelf([FromQuery] bool detailed = false)
        {
            if (detailed)
            {
                return await RunService(() => userService.GetSelf());
            }
            else
            {
                return await RunService(() => userService.GetSelfShort());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            return await RunService(() => userService.GetUser(id));
        }
        
        [AllowAnonymous]
        [HttpPost("activate")]
        public async Task<IActionResult> ActivateUser([FromBody] ActivateUserRequest request)
        {
            return await RunService(() => userService.ActivateUser(request));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest user)
        {
            return await RunService(() => userService.CreateUser(user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBase(string id, [FromBody] UpdateUserBaseInformationRequest request)
        {
            return await RunService(() => userService.UpdateBaseInformation(id, request));
        }

        [HttpPut("{id}/resetPassword")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            return await RunService(() => userService.ResetPassword(id));
        }

        [HttpPut("self/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            return await RunService(() => userService.ChangePassword(request));
        }

        [HttpPut("self/email")]
        public async Task<IActionResult> UpdateEmail([FromBody] string email)
        {
            return await RunService(() => userService.UpdateSelfEmail(email));
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(string id, [FromBody] ClientPermissions permissions)
        {
            return await RunService(() => userService.UpdatePermissions(id, permissions));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] ClientStatus status)
        {
            return await RunService(() => userService.UpdateStatus(id, status));
        }

        [HttpPut("{id}/accessLevel")]
        public async Task<IActionResult> UpdateAccessLevel(string id, [FromBody] int accessLevel)
        {
            return await RunService(() => userService.UpdateAccessLevel(id, accessLevel));
        }

        [HttpPut("{id}/scanRadius")]
        public async Task<IActionResult> UpdateScanRadius(string id, [FromBody] int scanRadius)
        {
            return await RunService(() => userService.UpdateScanRadius(id, scanRadius));
        }
        [HttpPut("{id}/updateLocationInterval")]
        public async Task<IActionResult> UpdateLocationInterval(string id, [FromBody] int updateLocationInterval)
        {
            return await RunService(() => userService.UpdateLocationSecondsInterval(id, updateLocationInterval));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return await RunService(() => userService.Delete(id));
        }
    }
}
