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
    public class ProjectReportServiceTests
    {
        private readonly Mock<IProjectReportRepository> _repo = new();
        private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        private ProjectReportService CreateService() => new ProjectReportService(_repo.Object, _memoryCache);

        [Fact]
        public async Task GetProjectTaskSummaryAsync_WhenNotCached_ShouldCallRepositoryAndCacheResult()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            var dto = new ProjectTaskSummaryDto
            {
                ProjectId = projectId,
                ProjectName = "Demo Project",
                TotalTasks = 10,
                TodoCount = 3,
                InProgressCount = 4,
                DoneCount = 3,
                OverdueCount = 1,
            };

            _repo.Setup(r => r.GetProjectTaskSummaryAsync(projectId, default)).ReturnsAsync(dto);

            var service = CreateService();

            // Act - 1. ยังไม่มี cache
            var result = await service.GetProjectTaskSummaryAsync(projectId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.ProjectId.Should().Be(projectId);
            _repo.Verify(r => r.GetProjectTaskSummaryAsync(projectId, default), Times.Once);

            // Act - 2. แบบมี cache
            var result2 = await service.GetProjectTaskSummaryAsync(projectId);

            // Assert - repository ถูกเรียกแค่ครั้งเดียว
            result2.IsSuccess.Should().BeTrue();
            result2.Data!.ProjectId.Should().Be(projectId);
            _repo.Verify(r => r.GetProjectTaskSummaryAsync(projectId, default), Times.Once);
        }

        [Fact]
        public async Task GetProjectTaskSummaryAsync_WhenRepositoryReturnsNull_ShouldReturnFailure()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _repo.Setup(r => r.GetProjectTaskSummaryAsync(projectId, default)).ReturnsAsync((ProjectTaskSummaryDto?)null);

            var service = CreateService();

            // Act
            var result = await service.GetProjectTaskSummaryAsync(projectId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Project not found or no tasks.");
        }
    }
}
