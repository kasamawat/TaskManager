using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Interfaces
{
    public interface IUserProjectOverviewRepository
    {
        Task<List<UserProjectOverviewDto>?> GetUserProjectOverviewAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
