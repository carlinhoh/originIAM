using Xunit;
using Moq;
using StackExchange.Redis;
using Newtonsoft.Json;
using OriginIAM.Domain.Entities;
using OriginIAM.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace OriginIAM.Infrastructure.Tests.Repositories
{
    public class EligibilityRepositoryTests
    {
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly EligibilityRepository _eligibilityRepository;

        public EligibilityRepositoryTests()
        {
            _mockDatabase = new Mock<IDatabase>();
            _eligibilityRepository = new EligibilityRepository(_mockDatabase.Object);
        }

        [Fact]
        public async Task GetAndDeleteUser_WithValidEmail_ReturnsUserAndDeletesFromRedis()
        {
            // Arrange
            var user = new User("test@example.com", "hashedpassword", "US", "1");
            string jsonValue = JsonConvert.SerializeObject(user);
            _mockDatabase.Setup(db => db.StringGetDeleteAsync(user.Email, CommandFlags.None))
                .ReturnsAsync(jsonValue);

            // Act
            var result = await _eligibilityRepository.GetAndDeleteUser(user.Email, user.PasswordHash);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            _mockDatabase.Verify(db => db.StringGetDeleteAsync(user.Email, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithValidEmail_DeletesUserFromRedis()
        {
            // Arrange
            var email = "test@example.com";
            _mockDatabase.Setup(db => db.KeyDeleteAsync(email, CommandFlags.None)).ReturnsAsync(true);

            // Act
            var result = await _eligibilityRepository.DeleteUser(email);

            // Assert
            Assert.True(result);
            _mockDatabase.Verify(db => db.KeyDeleteAsync(email, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task GetAndDeleteUser_WithNonExistingEmail_ReturnsNull()
        {
            // Arrange
            var email = "non-existing@example.com";
            _mockDatabase.Setup(db => db.StringGetDeleteAsync(email, CommandFlags.None)).ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _eligibilityRepository.GetAndDeleteUser(email, "anyPassword");

            // Assert
            Assert.Null(result);
            _mockDatabase.Verify(db => db.StringGetDeleteAsync(email, CommandFlags.None), Times.Once);
        }


    }
}
