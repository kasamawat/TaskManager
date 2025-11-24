using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class ProjectReportRepository : IProjectReportRepository
    {
        private readonly string _connectionString;

        public ProjectReportRepository(IConfiguration connectionString)
        {
            _connectionString = connectionString.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<ProjectTaskSummaryDto?> GetProjectTaskSummaryAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            const string storedProcName = "dbo.GetProjectTaskSummary";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var param = new
            {
                ProjectId = projectId,
            };

            var result = await connection.QuerySingleOrDefaultAsync<ProjectTaskSummaryDto>(
                storedProcName,
                param,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
