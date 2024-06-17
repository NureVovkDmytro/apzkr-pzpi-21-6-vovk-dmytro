using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Entity;
using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Authorize(Roles = "user")]
    [Route("api/robots")]
    [ApiController]
    public class RobotsController : BaseController
    {
        private readonly IRobotService robotService;

        public RobotsController(IRobotService robotService)
        {
            this.robotService = robotService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] RobotsSearchRequest request)
        {
            return await RunService(() => robotService.Search(request));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return await RunService(() => robotService.Get(id));
        }

        [HttpGet("{id}/token")]
        public async Task<IActionResult> GetToken(string id)
        {
            return await RunService(() => robotService.GetToken(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRobotRequest request)
        {
            return await RunService(() => robotService.CreateRobot(request));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRobotRequest request)
        {
            return await RunService(() => robotService.UpdateRobot(id, request));
        }

        [HttpPut("self/location")]
        public async Task<IActionResult> UpdateLocation([FromBody] GeoCoordinates location)
        {
            return await RunService(() => robotService.UpdateLocation(location));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] ClientStatus status)
        {
            return await RunService(() => robotService.UpdateStatus(id, status));
        }

        [HttpPut("{id}/accessLevel")]
        public async Task<IActionResult> UpdateAccessLevel(string id, [FromBody] int accessLevel)
        {
            return await RunService(() => robotService.UpdateAccessLevel(id, accessLevel));
        }

        [HttpPut("{id}/scanRadius")]
        public async Task<IActionResult> UpdateScanRadius(string id, [FromBody] int scanRadius)
        {
            return await RunService(() => robotService.UpdateScanRadius(id, scanRadius));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return await RunService(() => robotService.Delete(id));
        }
    }
}
