using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;

        public TaskRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ProjectTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.ProjectTasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        public async Task AddAsync(ProjectTask task, CancellationToken cancellationToken = default)
        {
            await _db.ProjectTasks.AddAsync(task, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateAsync(ProjectTask task, CancellationToken cancellationToken = default)
        {
            _db.ProjectTasks.Update(task);
            await _db.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(ProjectTask task, CancellationToken cancellationToken = default)
        {
            _db.ProjectTasks.Remove(task);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
