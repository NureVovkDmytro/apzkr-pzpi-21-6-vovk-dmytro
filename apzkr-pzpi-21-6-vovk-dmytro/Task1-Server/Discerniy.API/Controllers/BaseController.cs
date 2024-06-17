using Discerniy.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected async Task<IActionResult> RunService<T>(Func<Task<T>> service)
        {
            try
            {
                return Ok(await service());
            }
            catch (BadRequestException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (NotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
