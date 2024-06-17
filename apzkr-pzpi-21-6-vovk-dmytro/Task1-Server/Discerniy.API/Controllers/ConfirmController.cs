using Discerniy.Domain.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Route("api/confirm")]
    [ApiController]
    public class ConfirmController : BaseController
    {
        protected readonly IConfirmationManager configurationManager;

        public ConfirmController(IConfirmationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        [HttpGet]
        public async Task<IActionResult> Confirm([FromQuery] string token, [FromQuery] string type)
        {
            return await RunService(async () => await configurationManager.Confirm(type, token));
        }
    }
}
