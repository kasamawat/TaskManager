using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _repo = new();

        private ProjectService CreateService() => new ProjectService(_repo.Object);

        [Fact]
        public async Task Create_Should_Succeed()
        {
            var service = CreateService();

            var request = new CreateProjectRequest
            {
                Name = "Test Project",
            };

            var result = await service.CreateAsync(Guid.NewGuid(), request);

            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Test Project");

            _repo.Verify(x => x.AddAsync(It.IsAny<Project>(), default), Times.Once);
        }
        [Fact]
        public async Task Update_Should_Fail_When_NotFound()
        {
            _repo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Project?)null);

            var service = CreateService();

            var request = new UpdateProjectRequest { Name = "Updated" };

            var result = await service.UpdateAsync(Guid.NewGuid(), request);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Project not found.");
        }
    }
}
