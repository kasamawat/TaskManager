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
    public class UserProjectOverviewService : IUserProjectOverviewService
    {
        private readonly IUserProjectOverviewRepository _userProjectOverviewRepository;
        private readonly IMemoryCache _cache;

        public UserProjectOverviewService(IUserProjectOverviewRepository userProjectOverviewRepository, IMemoryCache cache) 
        {
            _userProjectOverviewRepository = userProjectOverviewRepository;
            _cache = cache;
        }

        public async Task<ServiceResult<List<UserProjectOverviewDto>>> GetUserProjectOverviewAsync(Guid userId)
        {
            // 1) check cache first
            var cachKey = $"user-project-overview:{userId}";
            if(_cache.TryGetValue<List<UserProjectOverviewDto>>(cachKey, out var cached))
            {
                return ServiceResult<List<UserProjectOverviewDto>>.Success(cached!);
            }
            
            // 2) if not have in cache -> get SP
            var data = await _userProjectOverviewRepository.GetUserProjectOverviewAsync(userId);
            if(data == null || data.Count == 0) 
            {
                return ServiceResult<List<UserProjectOverviewDto>>.Fail("No projects found for this user.");
            }

            // 3) save result to cache and set expired
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                SlidingExpiration = TimeSpan.FromSeconds(30),
            };

            _cache.Set(cachKey, data, options);

            return ServiceResult<List<UserProjectOverviewDto>>.Success(data);
        }
    }
}
