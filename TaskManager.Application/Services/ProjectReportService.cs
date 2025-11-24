using Microsoft.Extensions.Caching.Memory;
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
    public class ProjectReportService : IProjectReportService
    {
        private readonly IProjectReportRepository _reportRepository;
        private readonly IMemoryCache _cache;

        public ProjectReportService(IProjectReportRepository reportRepository, IMemoryCache cache)
        {
            _reportRepository = reportRepository;
            _cache = cache;
        }

        public async Task<ServiceResult<ProjectTaskSummaryDto>> GetProjectTaskSummaryAsync(Guid projectId)
        {
            var cacheKey = $"project-summary:{projectId}";
            
            // 1) check from cache first
            if(_cache.TryGetValue<ProjectTaskSummaryDto>(cacheKey, out var cached))
            {
                return ServiceResult<ProjectTaskSummaryDto>.Success(cached);
            }

            // 2) if not have in cache -> get SP
            var summary = await _reportRepository.GetProjectTaskSummaryAsync(projectId);
            if(summary is null)
            {
                return ServiceResult<ProjectTaskSummaryDto>.Fail("Project not found or no tasks.");
            }

            // 3) save result to cache and set expired
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                SlidingExpiration = TimeSpan.FromSeconds(30),
            };

            _cache.Set(cacheKey, summary, options);

            return ServiceResult<ProjectTaskSummaryDto>.Success(summary);
        }
    }
}
