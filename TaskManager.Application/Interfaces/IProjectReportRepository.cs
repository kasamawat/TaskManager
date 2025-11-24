using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Interfaces
{
    public interface IProjectReportRepository
    {
        Task<ProjectTaskSummaryDto?> GetProjectTaskSummaryAsync(Guid projectId, CancellationToken cancellationToken = default);
    }
}
