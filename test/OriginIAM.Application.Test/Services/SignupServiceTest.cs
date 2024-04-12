using FluentAssertions;
using Moq;
using OriginIAM.Application.Dtos;
using OriginIAM.Application.Services;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using Xunit;

namespace OriginIAM.Application.Tests.Services
{
    public class SignupServiceTests
    {
        private readonly Mock<IEligibilityService> _eligibilityServiceMock = new Mock<IEligibilityService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly SignupService _signupService;

        public SignupServiceTests()
        {
            _signupService = new SignupService(_eligibilityServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task SignupAsync_UserFromEligibility_CreatesUserSuccessfully()
        {
            // Arrange
            var request = new SignupDto { Email = "user@example.com", Password = "strongpassword", FullName = "Test User" };
            var user = new User(request.Email, request.Password, "Country", "EmployerId", request.FullName);

            _eligibilityServiceMock.Setup(m => m.GetAndDeleteUser(request.Email, request.Password)).ReturnsAsync(user);
            _userServiceMock.Setup(m => m.CreateUserAsync(It.IsAny<User>())).ReturnsAsync("newUserId");

            // Act
            var result = await _signupService.SignupAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.UserId.Should().Be("newUserId");
            result.Message.Should().Be("User created successfully! Enjoy the platform.");
        }

        [Fact]
        public async Task SignupAsync_UserNotFromEligibilityButExists_ReturnsUserAlreadyExists()
        {
            // Arrange
            var request = new SignupDto { Email = "existing@example.com", Password = "password", FullName = "Existing User" };
            var user = new User(request.Email, request.Password, "Country", "", request.FullName);

            _eligibilityServiceMock.Setup(m => m.GetAndDeleteUser(request.Email, request.Password)).ReturnsAsync((User)null);
            _userServiceMock.Setup(m => m.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);

            // Act
            var result = await _signupService.SignupAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("User already created.");
            result.UserId.Should().BeNull();
        }

        [Fact]
        public async Task SignupAsync_UserNotFromEligibility_CreatesUserSuccessfully()
        {
            // Arrange
            var request = new SignupDto { Email = "mario@example.com", Country = "BR", Password = "strongpassword", FullName = "New User" };

            _eligibilityServiceMock.Setup(m => m.GetAndDeleteUser(request.Email, request.Password)).ReturnsAsync((User)null);
            _userServiceMock.Setup(m => m.GetUserByEmailAsync(request.Email)).ReturnsAsync((User)null);
            _userServiceMock.Setup(m => m.CreateUserAsync(It.IsAny<User>())).ReturnsAsync("userId");

            // Act
            var result = await _signupService.SignupAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.UserId.Should().Be("userId");
            result.Message.Should().Be("User created successfully! Enjoy the platform.");
        }

        [Fact]
        public async Task SignupAsync_UserFromEligibility_DeletesUserFromEligibility()
        {
            // Arrange
            var request = new SignupDto { Email = "user@example.com", Password = "strongpassword", FullName = "Test User" };
            var user = new User(request.Email, request.Password, "Country", "EmployerId", request.FullName);

            _eligibilityServiceMock.Setup(m => m.GetAndDeleteUser(request.Email, request.Password)).ReturnsAsync(user);
            _userServiceMock.Setup(m => m.CreateUserAsync(It.IsAny<User>())).ReturnsAsync("newUserId");

            // Act
            var result = await _signupService.SignupAsync(request);

            // Assert
            _eligibilityServiceMock.Verify(m => m.DeleteUser(request.Email), Times.Once);
        }
    }
}
