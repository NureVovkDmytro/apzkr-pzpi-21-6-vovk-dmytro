using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelRequest login)
        {
            return await RunService(async () => await authService.Login(login));
        }

        [Authorize(Roles = "user")]
        [HttpGet("device/token")]
        public async Task<IActionResult> GenerateDeviceToken([FromQuery(Name = "userId")] string userId)
        {
            return await RunService(async () => await authService.GenerateDeviceToken(userId));
        }

        [Authorize(Roles = "device")]
        [HttpGet("device/refresh")]
        public async Task<IActionResult> RefreshDeviceToken()
        {
            return await RunService(authService.RefreshDeviceToken);
        }
    }
}
