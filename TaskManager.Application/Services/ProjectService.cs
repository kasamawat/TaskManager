using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        public Task<ServiceResult<ProjectDto>> CreateAsync(Guid userId, CreateProjectRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<bool>> DeleteAsync(Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ProjectDto>> UpdateAsync(Guid projectId, UpdateProjectRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
