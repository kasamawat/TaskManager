
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<IJwtTokenGenerator> _jwt = new();

        private AuthService CreateService() => new AuthService(_userRepo.Object, _hasher.Object, _jwt.Object);

        [Fact]
        public async Task Register_Should_Return_Success()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "123456"
            };

            _userRepo.Setup(x => x.GetByEmailAsync(request.Email, default)).ReturnsAsync((User?)null);

            _hasher.Setup(x => x.Hash("123456")).Returns("hashed123");

            _jwt.Setup(x => x.GenerateToken(It.IsAny<Guid>(), request.Email)).Returns("jwt-token");

            var service = CreateService();

            // Act
            var result = await service.RegisterAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Email.Should().Be(request.Email);
            result.Data.Token.Should().Be("jwt-token");

            _userRepo.Verify(x => x.AddAsync(It.IsAny<User>(), default), Times.Once);
        }
        [Fact]
        public async Task Register_Should_Fail_When_Email_Exists()
        {
            // Arrange
            var existing = new User("A", "B", "test@test.com", "hash");

            _userRepo.Setup(x => x.GetByEmailAsync("test@test.com", default)).ReturnsAsync(existing);

            var service = CreateService();

            var request = new RegisterRequest
            {
                Email = "test@test.com",
                Password = "123"
            };

            // Act
            var result = await service.RegisterAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Email already exists.");
        }
        [Fact]
        public async Task Login_Should_Succeed()
        {
            // Arrange
            var user = new User("A", "B", "a@test.com", "hashed");

            _userRepo.Setup(x => x.GetByEmailAsync("a@test.com", default)).ReturnsAsync(user);

            _hasher.Setup(x => x.VerifyPassword("hashed", "123")).Returns(true);

            _jwt.Setup(x => x.GenerateToken(user.Id, user.Email)).Returns("jwt-token");

            var service = CreateService();

            var request = new LoginRequest
            {
                Email = "a@test.com",
                Password = "123"
            };

            // Act
            var result = await service.LoginAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Token.Should().Be("jwt-token");
        }
    }
}
