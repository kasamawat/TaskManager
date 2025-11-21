using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ServiceResult<ProjectDto>> CreateAsync(Guid userId, CreateProjectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<ProjectDto>.Fail("Project name is required");
            }

            var project = new Project(
                name: request.Name,
                description: request.Description,
                ownerId: userId
            );

            await _projectRepository.AddAsync(project);

            var dto = MapToDto(project);

            return ServiceResult<ProjectDto>.Success(dto);
        }
        public async Task<ServiceResult<ProjectDto>> UpdateAsync(Guid projectId, UpdateProjectRequest request)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<ProjectDto>.Fail("Project not found.");
            }

            if(string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<ProjectDto>.Fail("Project name is required");
            }

            project.Rename(request.Name, request.Description);

            await _projectRepository.UpdateAsync(project);

            var dto = MapToDto(project);

            return ServiceResult<ProjectDto>.Success(dto);
        }
        public async Task<ServiceResult<bool>> DeleteAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if(project == null)
            {
                return ServiceResult<bool>.Fail("Project not found.");
            }

            await _projectRepository.DeleteAsync(project);

            return ServiceResult<bool>.Success(true);
        }

        private static ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
            };
        }
    }
}
