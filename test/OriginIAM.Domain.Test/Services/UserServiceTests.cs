using Xunit;
using Moq;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using OriginIAM.Domain.Services;

namespace OriginIAM.Domain.Test.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetUserByEmailAsync_ValidEmail_CallsRepositoryGetUserByEmailAsync()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(userRepositoryMock.Object);
            var email = "robertocarlos@example.com";

            // Act
            await service.GetUserByEmailAsync(email);

            // Assert
            userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ValidUser_CallsRepositoryAddAsync()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(userRepositoryMock.Object);
            var user = new User { Email = "neymar@example.com" };

            // Act
            await service.CreateUserAsync(user);

            // Assert
            userRepositoryMock.Verify(repo => repo.AddAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(userRepositoryMock.Object);
            var user = new User { Id = "123", Email = "ronaldinho@example.com" };

            // Act
            await service.UpdateUserAsync(user);

            // Assert
            userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task PatchUserDetailsAsync_ValidDetails_CallsRepositoryPatchUserDetailsAsync()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(userRepositoryMock.Object);
            var email = "edmundo@example.com";
            var country = "US";
            var salary = 10000.50m;

            // Act
            await service.PatchUserDetailsAsync(email, country, salary);

            // Assert
            userRepositoryMock.Verify(repo => repo.PatchUserDetailsAsync(email, country, salary), Times.Once);
        }

        [Fact]
        public async Task TerminateOldAccounts_ValidEmployerId_CallsRepositoryTerminateUsersByEmployerIdAsync()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(userRepositoryMock.Object);
            var employerId = "123";

            // Act
            await service.TerminateOldAccounts(employerId);

            // Assert
            userRepositoryMock.Verify(repo => repo.TerminateUsersByEmployerIdAsync(employerId), Times.Once);
        }

        [Fact]
        public async Task SetUserAsPendingByEmployerId_ValidEmployerId_CallsRepositorySetUserAsPendingByEmployerId()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(userRepositoryMock.Object);
            var employerId = "123";

            // Act
            await service.SetUserAsPendingByEmployerId(employerId);

            // Assert
            userRepositoryMock.Verify(repo => repo.SetUserAsPendingByEmployerId(employerId), Times.Once);
        }
    }
}
