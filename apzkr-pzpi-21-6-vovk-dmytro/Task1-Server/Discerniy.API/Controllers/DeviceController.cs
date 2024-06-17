using Discerniy.Domain.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Authorize]
    [Route("api/device")]
    [ApiController]
    public class DeviceController : BaseController
    {
        protected readonly IDeviceService deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            this.deviceService = deviceService;
        }

        [Authorize(Roles = "device")]
        [HttpGet("self")]
        public async Task<IActionResult> GetSelfInfo()
        {
            return await RunService(deviceService.GetDeviceInfo);
        }
    }
}
