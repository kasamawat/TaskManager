using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class UserProjectOverviewController : ControllerBase
    {
        private readonly IUserProjectOverviewService _userProjectOverviewService;
        
        public UserProjectOverviewController(IUserProjectOverviewService userProjectOverviewService)
        {
            _userProjectOverviewService = userProjectOverviewService;
        }

        // GET: /api/projects/{userId}/projects/overview
        [HttpGet("{userId:guid}/projects/overview")]
        public async Task<IActionResult> GetUserProjectOverview(Guid userId)
        {
            var result = await _userProjectOverviewService.GetUserProjectOverviewAsync(userId);
            if(!result.IsSuccess)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(result);
        }
    }
}
