using Xunit;
using Moq;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using OriginIAM.Domain.Services;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Test.Services
{
    public class EmployerServiceTests
    {
        [Fact]
        public async Task CreateEmployerAsync_ValidEmployer_CallsRepositoryAddAsync()
        {
            // Arrange
            var employerRepositoryMock = new Mock<IEmployerRepository>();
            var service = new EmployerService(employerRepositoryMock.Object);
            var employer = new Employer { Name = "Example Employer" };

            // Act
            await service.CreateEmployerAsync(employer);

            // Assert
            employerRepositoryMock.Verify(repo => repo.AddAsync(employer), Times.Once);
        }

        [Fact]
        public async Task GetEmployerIdByName_ValidName_CallsRepositoryGetEmployerIdByNameAsync()
        {
            // Arrange
            var employerRepositoryMock = new Mock<IEmployerRepository>();
            var service = new EmployerService(employerRepositoryMock.Object);
            var employerName = "Example Employer";

            // Act
            await service.GetEmployerIdByName(employerName);

            // Assert
            employerRepositoryMock.Verify(repo => repo.GetEmployerIdByNameAsync(employerName), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployerAsync_ValidEmployer_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var employerRepositoryMock = new Mock<IEmployerRepository>();
            var service = new EmployerService(employerRepositoryMock.Object);
            var employer = new Employer { Id = "123", Name = "Example Employer" };

            // Act
            await service.UpdateEmployerAsync(employer);

            // Assert
            employerRepositoryMock.Verify(repo => repo.UpdateAsync(employer), Times.Once);
        }
    }
}
