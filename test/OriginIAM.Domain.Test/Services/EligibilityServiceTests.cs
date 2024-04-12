using Xunit;
using Moq;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using OriginIAM.Domain.Services;

namespace OriginIAM.Domain.Test.Services
{
    public class EligibilityServiceTests
    {
        [Fact]
        public async Task DeleteUser_ValidEmail_CallsDeleteUser()
        {
            // Arrange
            var eligibilityRepositoryMock = new Mock<IEligibilityRepository>();
            var service = new EligibilityService(eligibilityRepositoryMock.Object);
            var email = "test@example.com";

            // Act
            await service.DeleteUser(email);

            // Assert
            eligibilityRepositoryMock.Verify(repo => repo.DeleteUser(email), Times.Once);
        }

        [Fact]
        public async Task GetAndDeleteUser_ValidCredentials_CallsGetAndDeleteUser()
        {
            // Arrange
            var eligibilityRepositoryMock = new Mock<IEligibilityRepository>();
            var service = new EligibilityService(eligibilityRepositoryMock.Object);
            var email = "test@example.com";
            var password = "password";

            // Act
            await service.GetAndDeleteUser(email, password);

            // Assert
            eligibilityRepositoryMock.Verify(repo => repo.GetAndDeleteUser(email, password), Times.Once);
        }

        [Fact]
        public async Task SaveOrUpdateEligibleUser_ValidUser_CallsSaveOrUpdateEligibleUser()
        {
            // Arrange
            var eligibilityRepositoryMock = new Mock<IEligibilityRepository>();
            var service = new EligibilityService(eligibilityRepositoryMock.Object);
            var user = new User { Email = "test@example.com", PasswordHash = "password" };

            // Act
            await service.SaveOrUpdateEligibleUser(user);

            // Assert
            eligibilityRepositoryMock.Verify(repo => repo.SaveOrUpdateEligibleUser(user), Times.Once);
        }
    }
}
