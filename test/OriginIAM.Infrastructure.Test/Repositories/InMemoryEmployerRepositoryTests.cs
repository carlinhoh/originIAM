using Xunit;
using OriginIAM.Domain.Entities;
using OriginIAM.Infrastructure.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace OriginIAM.Infrastructure.Tests.Repositories
{
    public class InMemoryEmployerRepositoryTests
    {
        private readonly InMemoryEmployerRepository _repository;

        public InMemoryEmployerRepositoryTests()
        {
            _repository = new InMemoryEmployerRepository();
        }

        [Fact]
        public async Task GetEmployerByNameAsync_ExistingName_ReturnsEmployer()
        {
            // Arrange
            var name = "Dijkstra";

            // Act
            var employer = await _repository.GetEmployerByNameAsync(name);

            // Assert
            Assert.NotNull(employer);
            Assert.Equal(name, employer.Name);
        }

        [Fact]
        public async Task GetEmployerByNameAsync_NonExistingName_ReturnsNull()
        {
            // Arrange
            var name = "NonExistingName";

            // Act
            var employer = await _repository.GetEmployerByNameAsync(name);

            // Assert
            Assert.Null(employer);
        }

        [Fact]
        public async Task AddAsync_AddsEmployerSuccessfully()
        {
            // Arrange
            var employer = new Employer("NewEmployer", "6");

            // Act
            await _repository.AddAsync(employer);
            var retrievedEmployer = await _repository.GetEmployerByNameAsync("NewEmployer");

            // Assert
            Assert.NotNull(retrievedEmployer);
            Assert.Equal("NewEmployer", retrievedEmployer.Name);
        }

        [Fact]
        public async Task GetEmployerIdByNameAsync_NonExistingName_ReturnsEmptyString()
        {
            // Arrange
            var name = "NonExistingName";

            // Act
            var employerId = await _repository.GetEmployerIdByNameAsync(name);

            // Assert
            Assert.Equal(string.Empty, employerId);
        }
    }
}
