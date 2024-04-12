using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using OriginIAM.Domain.Services;

namespace OriginIAM.Domain.Test.Services
{
    public class EligibilityReportServiceTests
    {
        [Fact]
        public async Task AddReportAsync_ValidReport_AddsReport()
        {
            // Arrange
            var reportRepositoryMock = new Mock<IEligibilityReportRepository>();
            var service = new EligibilityReportService(reportRepositoryMock.Object);
            var report = new EligibilityRecordReport();

            // Act
            await service.AddReportAsync(report);

            // Assert
            reportRepositoryMock.Verify(repo => repo.AddReportAsync(report), Times.Once);
        }

        [Fact]
        public async Task GetReportsByEmployerIdAsync_ValidId_ReturnsReports()
        {
            // Arrange
            var reportRepositoryMock = new Mock<IEligibilityReportRepository>();
            var service = new EligibilityReportService(reportRepositoryMock.Object);
            var employerId = "123";
            var expectedReports = new List<EligibilityRecordReport>();

            reportRepositoryMock.Setup(repo => repo.GetReportsByEmployerIdAsync(employerId))
                .ReturnsAsync(expectedReports);

            // Act
            var result = await service.GetReportsByEmployerIdAsync(employerId);

            // Assert
            Assert.Equal(expectedReports, result);
        }

        [Fact]
        public async Task RemoveLastReport_ValidId_RemovesLastReport()
        {
            // Arrange
            var reportRepositoryMock = new Mock<IEligibilityReportRepository>();
            var service = new EligibilityReportService(reportRepositoryMock.Object);
            var employerId = "123";

            // Act
            await service.RemoveLastReport(employerId);

            // Assert
            reportRepositoryMock.Verify(repo => repo.RemoveLastReport(employerId), Times.Once);
        }
    }
}
