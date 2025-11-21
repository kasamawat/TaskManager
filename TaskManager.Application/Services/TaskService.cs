using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }
        public async Task<ServiceResult<ProjectTaskDto>> CreateAsync(Guid projectId, CreateTaskRequest request)
        {
            // check project is have ?
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<ProjectTaskDto>.Fail("Project not found.");
            }

            // validate simple data
            if (string.IsNullOrWhiteSpace(request.Title)) 
            {
                return ServiceResult<ProjectTaskDto>.Fail("Task title is required.");
            }

            // create entity
            var task = new ProjectTask(
                title: request.Title,
                detail: request.Detail,
                dueDate: request.DueDate,
                projectId: projectId
            );

            await _taskRepository.AddAsync(task);

            var dto = MapToDto(task);

            return ServiceResult<ProjectTaskDto>.Success(dto);
        }
        public async Task<ServiceResult<ProjectTaskDto>> UpdateAsync(Guid taskId, UpdateTaskRequest request)
        {
            
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                return ServiceResult<ProjectTaskDto>.Fail("Task not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return ServiceResult<ProjectTaskDto>.Fail("Task Title is required.");
            }

            task.Update(
                title: request.Title,
                detail: request.Detail,
                dueDate: request.DueDate
            );

            await _taskRepository.UpdateAsync(task);

            var dto = MapToDto(task);

            return ServiceResult<ProjectTaskDto>.Success(dto);
        }
        public async Task<ServiceResult<bool>> ChangeStatusAsync(Guid taskId, TaskStatus status)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                return ServiceResult<bool>.Fail("Task not found.");
            }

            task.ChangeStatus(status);

            await _taskRepository.UpdateAsync(task);

            return ServiceResult<bool>.Success(true);
        }

        private static ProjectTaskDto MapToDto(ProjectTask task)
        {
            return new ProjectTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Detail = task.Detail,
                DueDate = task.DueDate,
                Status = task.Status,
            };
        }
    }
}
