using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Tasks;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<ServiceResult<ProjectTaskDto>> CreateAsync(Guid projectId, CreateTaskRequest request);
        Task<ServiceResult<ProjectTaskDto>> UpdateAsync(Guid taskId, UpdateTaskRequest request);
        Task<ServiceResult<bool>> ChangeStatusAsync(Guid taskId, TaskStatus status);
    }
}
