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
    public class UserProjectOverviewRepository : IUserProjectOverviewRepository
    {
        private readonly string _connectionString;

        public UserProjectOverviewRepository(IConfiguration connectionString)
        {
            _connectionString = connectionString.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<UserProjectOverviewDto>?> GetUserProjectOverviewAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            const string storedProcName = "dbo.GetUserProjectOverview";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var param = new
            {
                UserId = userId,
            };

            var command = new CommandDefinition(
                storedProcName,
                param,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            var result = await connection.QueryAsync<UserProjectOverviewDto>(command);

            return result.ToList();
        }
    }
}
