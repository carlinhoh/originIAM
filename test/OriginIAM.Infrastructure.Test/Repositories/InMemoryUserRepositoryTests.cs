using Xunit;
using OriginIAM.Infrastructure.Repositories;
using OriginIAM.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace OriginIAM.Infrastructure.Tests.Repositories
{
    public class InMemoryUserRepositoryTests
    {
        private readonly InMemoryUserRepository _userRepository;

        public InMemoryUserRepositoryTests()
        {
            _userRepository = new InMemoryUserRepository();
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "daniel93@dijkstra.com";

            // Act
            var user = await _userRepository.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@dijkstra.com";

            // Act
            var user = await _userRepository.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task AddAsync_UserIsAdded()
        {
            // Arrange
            var newUser = new User("newuser@dijkstra.com", "hashedpassword", "CA", "2");

            // Act
            var userId = await _userRepository.AddAsync(newUser);

            // Assert
            var addedUser = await _userRepository.GetUserByEmailAsync(newUser.Email);
            Assert.NotNull(addedUser);
            Assert.Equal("CA", addedUser.Country);
        }

        [Fact]
        public async Task UpdateAsync_UserIsUpdated()
        {
            // Arrange
            var email = "derekcooper@dijkstra.com";
            var userToUpdate = await _userRepository.GetUserByEmailAsync(email);
            userToUpdate.Country = "DE";

            // Act
            await _userRepository.UpdateAsync(userToUpdate);

            // Assert
            var updatedUser = await _userRepository.GetUserByEmailAsync(email);
            Assert.Equal("DE", updatedUser.Country);
        }

        [Fact]
        public async Task DeleteAsync_UserIsDeleted()
        {
            // Arrange
            var email = "removed_0@dijkstra.com";
            var userToDelete = await _userRepository.GetUserByEmailAsync(email);

            // Act
            await _userRepository.DeleteAsync(userToDelete.Email);

            // Assert
            var result = await _userRepository.GetUserByEmailAsync(email);
            Assert.Null(result);
        }

        [Fact]
        public async Task TerminateUsersByEmployerIdAsync_UsersAreTerminated()
        {
            // Arrange
            var employerId = "1";

            // Act
            await _userRepository.TerminateUsersByEmployerIdAsync(employerId);

            // Assert
            var users = _userRepository._users.Values.Where(u => u.EmployerId == employerId && !u.VerifiedEmployee);
            Assert.All(users, user => Assert.False(user.VerifiedEmployee));
        }


        [Fact]
        public async Task SetUserAsPendingByEmployerId_UsersAreSetAsPending()
        {
            // Arrange
            var employerId = "1";
            await _userRepository.SetUserAsPendingByEmployerId(employerId);

            // Act
            var users = _userRepository._users.Values.Where(u => u.EmployerId == employerId && !u.VerifiedEmployee).ToList();

            // Assert
            Assert.NotEmpty(users);
            Assert.All(users, user => Assert.False(user.VerifiedEmployee));
        }

        [Fact]
        public async Task PatchUserDetailsAsync_UserDetailsArePatched()
        {
            // Arrange
            var email = "daniel93@dijkstra.com";
            var newCountry = "DE";
            var newSalary = 100000m;

            // Act
            var result = await _userRepository.PatchUserDetailsAsync(email, newCountry, newSalary);

            // Assert
            Assert.True(result);
            var user = await _userRepository.GetUserByEmailAsync(email);
            Assert.Equal(newCountry, user.Country);
            Assert.Equal(newSalary, user.Salary);
        }
    }
}
