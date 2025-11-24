using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectReportController : ControllerBase
    {
        private readonly IProjectReportService _projectReportService;

        public ProjectReportController(IProjectReportService projectReportService)
        {
            _projectReportService = projectReportService;
        }

        // GET: /api/projects/{projectId}/summary
        [HttpGet("{projectId:guid}/summary")]
        public async Task<IActionResult> GetProjectSummary(Guid projectId)
        {
            var result = await _projectReportService.GetProjectTaskSummaryAsync(projectId);
            if(!result.IsSuccess)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Data);
        }
    }
}
