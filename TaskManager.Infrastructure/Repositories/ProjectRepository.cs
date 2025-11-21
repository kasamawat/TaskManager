using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _db;
        public ProjectRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
        {
            await _db.Projects.AddAsync(project, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
        {
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
        {
            _db.Projects.Update(project);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
