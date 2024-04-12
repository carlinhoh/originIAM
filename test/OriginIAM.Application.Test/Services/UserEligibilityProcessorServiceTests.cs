using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Models;
using OriginIAM.Application.Services;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OriginIAM.Application.Tests.Services
{
    public class UserEligibilityProcessorServiceTests
    {
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IEmployerService> _employerServiceMock = new();
        private readonly Mock<IEligibilityReportService> _eligibilityReportServiceMock = new();
        private readonly Mock<IEligibilityService> _eligibilityServiceMock = new();
        private readonly UserEligibilityProcessorService _service;
        private readonly Mock<ILogger<UserEligibilityProcessorService>> _mockLogger = new Mock<ILogger<UserEligibilityProcessorService>>();

        public UserEligibilityProcessorServiceTests()
        {
            _service = new UserEligibilityProcessorService(
                _userServiceMock.Object,
                _employerServiceMock.Object,
                _eligibilityReportServiceMock.Object,
                _eligibilityServiceMock.Object, _mockLogger.Object);
        }

        [Theory]
        [InlineData("invalid@example.com", "", "US", "1990-01-01", -60000)] 
        [InlineData("invalid@example.com", "Test User", "", "1990-01-01", 60000)] 
        [InlineData("invalid@example.com", "Test User", "US", "1990-01-01", -1000)] 
        [InlineData("", "Test User", "US", "1990-01-01", 60000)] 
        [InlineData("", "", "", "1990-01-01", 00000)] 
        public async Task ProcessUserEligibilityAsync_InvalidRecordInputs_Fails(string email, string fullName, string country, string birthDate, decimal salary)
        {
            // Arrange
            var record = new EligibilityRecord
            {
                Email = email,
                FullName = fullName,
                Country = country,
                BirthDate = DateTime.Parse(birthDate),
                Salary = salary
            };
            var employerId = "employerId";

            // Act
            await _service.ProcessUserEligibilityAsync(record, employerId);

            // Assert
            _eligibilityServiceMock.Verify(x => x.SaveOrUpdateEligibleUser(It.IsAny<User>()), Times.Never);
            _eligibilityReportServiceMock.Verify(x => x.AddReportAsync(It.IsAny<EligibilityRecordReport>()), Times.Never);
            record.ProcessSuccess.Should().BeFalse();
            record.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ProcessUserEligibilityAsync_WithInvalidRecord_SetsRecordAsFailed()
        {
            // Arrange
            var record = new EligibilityRecord { Email = "invalid@example.com" }; 
            var employerId = "employerId";

            // Act
            await _service.ProcessUserEligibilityAsync(record, employerId);

            // Assert
            Assert.False(record.ProcessSuccess);
            Assert.NotEmpty(record.Errors);
        }

        [Fact]
        public async Task ProcessUserEligibilityAsync_WithValidRecordAndNewUser_CreatesUserAndReportsSuccess()
        {
            // Arrange
            var record = new EligibilityRecord
            {
                Email = "Richarhamming@example.com",
                FullName = "Richard Hamming",
                Country = "US",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                Salary = 60000
            };
            var employerId = "employerId";

            _userServiceMock.Setup(x => x.GetUserByEmailAsync(record.Email)).ReturnsAsync((User)null);

            // Act
            await _service.ProcessUserEligibilityAsync(record, employerId);

            // Assert
            _eligibilityServiceMock.Verify(x => x.SaveOrUpdateEligibleUser(It.IsAny<User>()), Times.Once);
            _eligibilityReportServiceMock.Verify(x => x.AddReportAsync(It.IsAny<EligibilityRecordReport>()), Times.Once);
            Assert.True(record.ProcessSuccess);
        }

        [Fact]
        public async Task ProcessUserEligibilityAsync_WithValidRecordAndExistingUser_UpdatesUserDetails()
        {
            // Arrange
            var record = new EligibilityRecord
            {
                Email = "donaldknuth@example.com",
                FullName = "Donald Knuth",
                Country = "US",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                Salary = 60000
            };
            var employerId = "employerId";
            var existingUser = new User(record.Email, record.FullName, record.Country, record.BirthDate.Value, record.Salary.Value, employerId);

            _userServiceMock.Setup(x => x.GetUserByEmailAsync(record.Email)).ReturnsAsync(existingUser);

            // Act
            await _service.ProcessUserEligibilityAsync(record, employerId);

            // Assert
            _userServiceMock.Verify(x => x.PatchUserDetailsAsync(record.Email, record.Country, record.Salary), Times.Once);
            _eligibilityReportServiceMock.Verify(x => x.AddReportAsync(It.IsAny<EligibilityRecordReport>()), Times.Once);
            Assert.True(record.ProcessSuccess);
        }

        [Fact]
        public async Task ProcessUserEligibilityAsync_RecordValidationFails_SkipsCreationAndUpdate()
        {
            // Arrange
            var record = new EligibilityRecord
            {
                Email = "fail@example.com",
                FullName = "Test User",
                Country = "InvalidCountry",
                BirthDate = DateTime.UtcNow,
                Salary = -100 
            };
            var employerId = "employerId";
            
            _userServiceMock.Setup(x => x.GetUserByEmailAsync(record.Email)).ReturnsAsync((User)null);

            // Act
            await _service.ProcessUserEligibilityAsync(record, employerId);

            // Assert
            _eligibilityServiceMock.Verify(x => x.SaveOrUpdateEligibleUser(It.IsAny<User>()), Times.Never);
            _eligibilityReportServiceMock.Verify(x => x.AddReportAsync(It.IsAny<EligibilityRecordReport>()), Times.Never);
            record.ProcessSuccess.Should().BeFalse();
            record.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetEmployerIdByName_WithValidEmployerName_ReturnsEmployerId()
        {
            // Arrange
            var employerName = "ValidEmployer";
            var expectedEmployerId = "expectedEmployerId";
            _employerServiceMock.Setup(m => m.GetEmployerIdByName(employerName)).ReturnsAsync(expectedEmployerId);

            // Act
            var result = await _service.GetEmployerIdByName(employerName);

            // Assert
            result.Should().Be(expectedEmployerId);
        }

        [Fact]
        public async Task SetUserAsPendingConfirmation_WithValidEmployerId_ReturnsTrue()
        {
            // Arrange
            var employerId = "employerId";
            _userServiceMock.Setup(m => m.SetUserAsPendingByEmployerId(employerId)).ReturnsAsync(true);

            // Act
            var result = await _service.SetUserAsPendingConfirmation(employerId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task TerminateOldAccounts_WithValidEmployerId_ReturnsTrue()
        {
            // Arrange
            var employerId = "employerId";
            _userServiceMock.Setup(m => m.TerminateOldAccounts(employerId)).ReturnsAsync(true);

            // Act
            var result = await _service.TerminateOldAccounts(employerId);

            // Assert
            result.Should().BeTrue();
        }
    }
}
