using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Authorize]
    [Route("api/system")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        [HttpGet("time")]
        public IActionResult Time()
        {
            return Ok(Math.Round(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
        }
    }
}
