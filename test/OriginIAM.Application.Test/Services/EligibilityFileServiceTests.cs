using Moq;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Models;
using OriginIAM.Application.Services;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using OriginIAM.Domain.Interfaces;

namespace OriginIAM.Application.Services.Tests
{
    public class EligibilityFileServiceTests
    {
        private readonly Mock<IFileDownloader> _fileDownloaderMock = new();
        private readonly Mock<ICsvParser<EligibilityRecord>> _csvParserMock = new();
        private readonly Mock<IUserEligibilityProcessor> _userEligibilityProcessorMock = new();
        private readonly Mock<IEligibilityReportService> _eligibilityReportServiceMock = new();
        private readonly EligibilityFileService _service;

        public EligibilityFileServiceTests()
        {
            _service = new EligibilityFileService(
                _fileDownloaderMock.Object,
                _csvParserMock.Object,
                _userEligibilityProcessorMock.Object,
                _eligibilityReportServiceMock.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ProcessFileAsync_ProcessesFilesBasedOnCompanySize(bool isSmallCompany)
        {
            // Arrange
            var blobUrl = $"http://example.com/{(isSmallCompany ? "small" : "large")}File.csv";
            var employerName = isSmallCompany ? "SmallCompany" : "LargeCompany";
            string contentType = "text/csv";

            _fileDownloaderMock.Setup(m => m.DownloadFileAsync(blobUrl, It.IsAny<Func<Stream, Task>>(), contentType))
                .Returns(async (string url, Func<Stream, Task> process, string mediaType) =>
                {
                    using var stream = new MemoryStream();
                    await process(stream);
                });

            var records = new List<EligibilityRecord>
                        {
                            new EligibilityRecord(),
                            new EligibilityRecord()
                        };

            _csvParserMock.Setup(m => m.ParseCsvAsync(It.IsAny<Stream>()))
                .Returns(GetAsyncEnumerable(records));

            // Act
            await _service.ProcessFileAsync(blobUrl, employerName, isSmallCompany);

            // Assert
            _fileDownloaderMock.Verify(m => m.DownloadFileAsync(blobUrl, It.IsAny<Func<Stream, Task>>(), "text/csv"), Times.Once);
            _csvParserMock.Verify(m => m.ParseCsvAsync(It.IsAny<Stream>()), Times.Once);
            _userEligibilityProcessorMock.Verify(m => m.ProcessUserEligibilityAsync(It.IsAny<EligibilityRecord>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessFileAsync_FailureToDownloadFile_ThrowsException()
        {
            // Arrange
            var blobUrl = "http://example.com/failToDownload.csv";
            var employerName = "CompanyWithError";
            var isSmallCompany = true;

            _fileDownloaderMock.Setup(m => m.DownloadFileAsync(blobUrl, It.IsAny<Func<Stream, Task>>(), "text/csv"))
                               .ThrowsAsync(new IOException("Failed to download file."));

            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() => _service.ProcessFileAsync(blobUrl, employerName, isSmallCompany));

            _fileDownloaderMock.Verify(m => m.DownloadFileAsync(blobUrl, It.IsAny<Func<Stream, Task>>(), "text/csv"), Times.Once);
        }

        [Fact]
        public async Task GetEligibilityFileReport_ReturnsPaginatedResult()
        {
            // Arrange
            var employerId = "123";
            var pageNumber = 1;
            var pageSize = 10;
            var expectedReport = new PaginatedResult<EligibilityRecordReport>();

            _eligibilityReportServiceMock.Setup(s => s.GetReportsByEmployerNameAsync(employerId, pageNumber, pageSize))
                .ReturnsAsync(expectedReport);

            // Act
            var result = await _service.GetEligibilityFileReport(employerId, pageNumber, pageSize);

            // Assert
            Assert.Equal(expectedReport, result);
            _eligibilityReportServiceMock.Verify(s => s.GetReportsByEmployerNameAsync(employerId, pageNumber, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetEligibilityFileReport_InvalidPageNumberOrSize_ReturnsEmptyResult()
        {
            // Arrange
            var employerId = "invalid";
            var pageNumber = -1;
            var pageSize = 0; 
            var expectedReport = new PaginatedResult<EligibilityRecordReport>(); 

            _eligibilityReportServiceMock.Setup(s => s.GetReportsByEmployerNameAsync(employerId, pageNumber, pageSize))
                .ReturnsAsync(expectedReport);

            // Act
            var result = await _service.GetEligibilityFileReport(employerId, pageNumber, pageSize);

            // Assert
            Assert.Equal(expectedReport, result);
            _eligibilityReportServiceMock.Verify(s => s.GetReportsByEmployerNameAsync(employerId, pageNumber, pageSize), Times.Once);
        }

        private async IAsyncEnumerable<T> GetAsyncEnumerable<T>(IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return item;
            }
        }
    }
}
