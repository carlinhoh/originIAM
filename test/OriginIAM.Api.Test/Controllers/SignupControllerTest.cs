using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using OriginIAM.Api.Dtos.Request;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Dtos;
using OriginIAM.Application.Models;

namespace OriginIAM.Api.Controllers.Test
{
    public class SignupControllerTests
    {
        private readonly Mock<ISignupService> _signupServiceMock;
        private readonly SignupController _controller;

        public SignupControllerTests()
        {
            _signupServiceMock = new Mock<ISignupService>();
            _controller = new SignupController(_signupServiceMock.Object);
        }

        [Fact]
        public async Task Signup_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            var request = new SignupRequestDto();

            // Act
            var result = await _controller.Signup(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Signup_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new SignupRequestDto { Email = "test@example.com", Password = "Password123!" };
            var signupResult = new SignupResult { Success = true, UserId = "1" };

            _signupServiceMock.Setup(x => x.SignupAsync(It.IsAny<SignupDto>()))
                              .ReturnsAsync(signupResult);

            // Act
            var result = await _controller.Signup(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetUser", createdAtActionResult.ActionName);
            Assert.Equal(signupResult.UserId, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Signup_FailedSignup_ReturnsBadRequest()
        {
            // Arrange
            var request = new SignupRequestDto { Email = "test@example.com", Password = "Password123!" };
            var signupResult = new SignupResult { Success = false, Errors = new List<string> { "Email already exists" } };

            _signupServiceMock.Setup(x => x.SignupAsync(It.IsAny<SignupDto>()))
                              .ReturnsAsync(signupResult);

            // Act
            var result = await _controller.Signup(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(signupResult, badRequestResult.Value);
        }

        [Fact]
        public async Task Signup_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new SignupRequestDto { Email = "test@example.com", Password = "Password123!" };

            _signupServiceMock.Setup(x => x.SignupAsync(It.IsAny<SignupDto>()))
                              .ThrowsAsync(new Exception("Internal Server Error"));

            // Act
            var result = await _controller.Signup(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}