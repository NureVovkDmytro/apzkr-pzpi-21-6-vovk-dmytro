using Discerniy.Domain.Interface.Services;
using Discerniy.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discerniy.API.Controllers
{
    [Route("api/mark")]
    [Authorize(Roles = "user")]
    [ApiController]
    public class MarkController : BaseController
    {
        protected readonly IMarkService markService;

        public MarkController(IMarkService markService)
        {
            this.markService = markService;
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetMarksFromGroup(string groupId)
        {
            var marks = await markService.GetMarksFromGroup(groupId);
            return Ok(marks);
        }

        [HttpGet("{markId}")]
        public async Task<IActionResult> GetMark(string markId)
        {
            var mark = await markService.GetMark(markId);
            return Ok(mark);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMark([FromBody] CreateMarkRequest request)
        {
            var mark = await markService.CreateMark(request);
            return Ok(mark);
        }

        [HttpPut("{markId}")]
        public async Task<IActionResult> UpdateMark(string markId, [FromBody] UpdateMarkRequest request)
        {
            var mark = await markService.UpdateMark(markId, request);
            return Ok(mark);
        }

        [HttpDelete("{markId}")]
        public async Task<IActionResult> DeleteMark(string markId)
        {
            var mark = await markService.DeleteMark(markId);
            return Ok(mark);
        }
    }
}
