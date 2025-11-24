
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepo = new();
        private readonly Mock<IProjectRepository> _projectRepo = new();

        private TaskService CreateService() => new TaskService(_taskRepo.Object, _projectRepo.Object);

        [Fact]
        public async Task Create_Should_Fail_When_Project_Not_Found()
        {
            _projectRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Project?)null);

            var service = CreateService();

            var request = new CreateTaskRequest
            {
                Title = "Test",
            };

            var result = await service.CreateAsync(Guid.NewGuid(), request);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Project not found.");
        }
        [Fact]
        public async Task ChangeStatus_Should_Succeed()
        {
            var task = new ProjectTask("Demo", "Detail", null, Guid.NewGuid());

            _taskRepo.Setup(x => x.GetByIdAsync(task.Id, default)).ReturnsAsync(task);

            var service = CreateService();

            var result = await service.ChangeStatusAsync(task.Id, TaskStatus.Done);

            result.IsSuccess.Should().BeTrue();
            task.Status.Should().Be(TaskStatus.Done);
        }
    }
}
