using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        public Task<ServiceResult<bool>> ChangeStatusAsync(Guid taskId, TaskStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ProjectTaskDto>> CreateAsync(Guid projectId, CreateTaskRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ProjectTaskDto>> UpdateAsync(Guid taskId, UpdateTaskRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
