using Microsoft.AspNetCore.Mvc;
using Moq;
using OriginIAM.Application.Interfaces;
using OriginIAM.Api.Controllers;
using Xunit;
using OriginIAM.Api.Dtos.Request;
using OriginIAM.Api.Models.Response;
using OriginIAM.Application.Services;
using OriginIAM.Domain.Common;
using OriginIAM.Domain.Entities;

namespace OriginIAM.Api.Controllers.Test
{
    public class EligibilityFileControllerTests
    {
        private readonly Mock<IEligibilityFileService> _eligibilityFileServiceMock = new();
        private readonly EligibilityFileController _controller;

        public EligibilityFileControllerTests()
        {
            _controller = new EligibilityFileController(_eligibilityFileServiceMock.Object);
        }

        [Fact]
        public async Task UploadFile_WhenModelStateIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("FileAddress", "The file URL is required.");

            var requestDto = new EligibilityFileRequestDto
            {
                EmployerName = "Test Employer"
            };

            // Act
            var result = await _controller.UploadFile(requestDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsAssignableFrom<ErrorResponse>(badRequestResult.Value);
            Assert.Contains("The file URL is required.", errorResponse.Details);
        }

        [Fact]
        public async Task GetReportsByEmployerName_WhenPageNumberOrPageSizeIsInvalid_ReturnsBadRequest()
        {
            var employerName = "Invalid";
            var pageNumber = 0; // Invalid pageNumber
            var pageSize = 0;  // Invalid pageSize

            var result = await _controller.GetReportsByEmployerName(employerName, pageNumber, pageSize);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsAssignableFrom<ErrorResponse>(badRequestResult.Value);
            Assert.Contains("PageNumber and PageSize must be greater than 0.", errorResponse.Details);
        }

        [Fact]
        public async Task GetReportsByEmployerName_WhenNoReportsFound_ReturnsNotFound()
        {
            var employerName = "NonExistent";
            var pageNumber = 1;
            var pageSize = 10;

            _eligibilityFileServiceMock.Setup(m => m.GetEligibilityFileReport(employerName.ToUpper(), pageNumber, pageSize))
                .ReturnsAsync(new PaginatedResult<EligibilityRecordReport> { Items = new List<EligibilityRecordReport>() });

            var result = await _controller.GetReportsByEmployerName(employerName, pageNumber, pageSize);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"No reports found for employer: {employerName}.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetReportsByEmployerName_WhenReportsAreAvailable_ReturnsOkWithResults()
        {
            var employerName = "ExistTestEmployer";
            var pageNumber = 1;
            var pageSize = 10;

            var paginatedResult = new PaginatedResult<EligibilityRecordReport>
            {
                Items = new List<EligibilityRecordReport> { new EligibilityRecordReport(), new EligibilityRecordReport() },
                TotalCount = 2,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _eligibilityFileServiceMock.Setup(m => m.GetEligibilityFileReport(employerName.ToUpper(), pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);

            var result = await _controller.GetReportsByEmployerName(employerName, pageNumber, pageSize);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<PaginatedResult<EligibilityRecordReport>>(okResult.Value);
            Assert.Equal(2, resultValue.TotalCount);
        }


    }
}