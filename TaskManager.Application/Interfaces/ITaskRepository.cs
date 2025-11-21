using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(ProjectTask task, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProjectTask task, CancellationToken cancellationToken = default);
        Task DeleteAsync(ProjectTask task, CancellationToken cancellationToken = default);
    }
}
