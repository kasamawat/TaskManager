using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // POST: /api/projects/{projectId}/tasks
        [HttpPost("projects/{projectId:guid}/tasks")]
        public async Task<IActionResult> CreateTask(Guid projectId ,[FromBody] CreateTaskRequest request)
        {
            var result = await _taskService.CreateAsync(projectId, request);
            if(!result.IsSuccess)
            {
                return BadRequest(new { error= result.Error });
            }

            return Ok(result.Data);
        }

        // PUT: /api/tasks/{taskId}
        [HttpPut("tasks/{taskId:guid}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskRequest request)
        {
            var result = await _taskService.UpdateAsync(taskId, request);

            if(!result.IsSuccess)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        // PATCH: /api/{taskId}/status
        [HttpPatch("tasks/{taskId:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid taskId, [FromBody] ChangeTaskStatusRequest request)
        {
            var result = await _taskService.ChangeStatusAsync(taskId, request.Status);

            if(!result.IsSuccess)
            {
                return NotFound( new { error = result.Error });
            }

            return NoContent();
        }
    }
}
