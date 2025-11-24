using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;

namespace TaskManager.Tests
{
    public class UserProjectOverviewServiceTests
    {
        private readonly Mock<IUserProjectOverviewRepository> _repo = new();
        private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        private UserProjectOverviewService CreateService() => new UserProjectOverviewService(_repo.Object, _memoryCache);

        [Fact]
        public async Task GetUserProjectOverviewAsync_WhenNotCached_ShouldCallRepositoryAndCacheResult()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var list = new List<UserProjectOverviewDto>
            {
                new()
                {
                    ProjectId = Guid.NewGuid(),
                    ProjectName = "Proj A",
                    TotalTasks = 5,
                    TodoCount = 2,
                    InProgressCount = 2,
                    DoneCount = 1,
                    OverdueCount = 0
                }
            };

            _repo.Setup(r => r.GetUserProjectOverviewAsync(userId, default)).ReturnsAsync(list);

            var service = CreateService();

            // Act - 1. ไม่มี cache
            var result = await service.GetUserProjectOverviewAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Count.Should().Be(1);
            _repo.Verify(r => r.GetUserProjectOverviewAsync(userId, default), Times.Once);

            // Act - 2. มี cache
            var result2 = await service.GetUserProjectOverviewAsync(userId);

            // Assert - repository ถูกเรียกแค่ครั้งเดียว
            result2.IsSuccess.Should().BeTrue();
            result2.Data!.Count.Should().Be(1);
            _repo.Verify(r => r.GetUserProjectOverviewAsync(userId, default), Times.Once);
        }

        [Fact]
        public async Task GetUserProjectOverviewAsync_WhenRepositoryReturnsNullOrEmpty_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repo.Setup(r => r.GetUserProjectOverviewAsync(userId, default)).ReturnsAsync(new List<UserProjectOverviewDto>());

            var service = CreateService();

            // Act
            var result = await service.GetUserProjectOverviewAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("No projects found for this user.");
        }
    }
}
