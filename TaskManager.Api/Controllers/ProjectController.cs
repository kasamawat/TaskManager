using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // POST: /api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            var userId = GetUserIdFromToken();
            if(userId == null) 
            {
                return Unauthorized(new { error = "Invalid token." });
            }

            var result = await _projectService.CreateAsync(userId.Value, request);
            if(!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(result);
        }
        [HttpPut("{projectId:guid}")]
        // PUT: /api/project/update
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] UpdateProjectRequest request)
        {
            var result = await _projectService.UpdateAsync(projectId, request);
            if(!result.IsSuccess)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        // DELETE: /api/project/delete
        [HttpDelete("{projectId:guid}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var result = await _projectService.DeleteAsync(projectId);
            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.Error});
            }

            return NoContent();
        }

        // Pull userId from JWT (sub claim)
        private Guid? GetUserIdFromToken()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if(sub == null)
            {
                return null;
            }

            if(Guid.TryParse(sub, out var id))
            {
                return id;
            }

            return null;
        }
    }
}
