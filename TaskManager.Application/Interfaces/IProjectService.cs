using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Interfaces
{
    public interface IProjectService
    {
        Task<ServiceResult<ProjectDto>> CreateAsync(Guid userId, CreateProjectRequest request);
        Task<ServiceResult<ProjectDto>> UpdateAsync(Guid projectId, UpdateProjectRequest request);
        Task<ServiceResult<bool>> DeleteAsync(Guid projectId);
    }
}
