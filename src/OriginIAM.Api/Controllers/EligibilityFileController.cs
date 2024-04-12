using Microsoft.AspNetCore.Mvc;
using OriginIAM.Api.Dtos.Request;
using OriginIAM.Api.Dtos.Response;
using OriginIAM.Api.Mappers;
using OriginIAM.Api.Models.Response;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Models;
using OriginIAM.Domain.Common;

namespace OriginIAM.Api.Controllers
{
    /// <summary>
    /// Controller for handling eligibility file operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EligibilityFileController : ControllerBase
    {
        private readonly IEligibilityFileService _eligibilityFileService;

        public EligibilityFileController(IEligibilityFileService eligibilityFileService)
        {
            _eligibilityFileService = eligibilityFileService;
        }

        /// <summary>
        /// Uploads an eligibility file and processes it.
        /// </summary>
        /// <param name="request">The eligibility file request details.</param>
        /// <returns>A status indicating the result of the upload operation.</returns>
        /// <response code="202">Returns the link to consult the report in real time.</response>
        /// <response code="400">If the request is invalid, returns a list of errors.</response>
        /// <response code="500">If an internal error occurs, returns an error message.</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(EligibilityBaseFileResponseDto), 202)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UploadFile([FromBody] EligibilityFileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                return BadRequest(new ErrorResponse { Details = errors });
            }
            try
            {
                var task = _eligibilityFileService.ProcessFileAsync(request.FileAddress, request.EmployerName, false);

                var resultLink = Url.Link("GetReportByEmployerName", new { employerName = request.EmployerName, pageNumber = 1, pageSize = 10 });

                var responseDto = EligibilityResponseFactory.CreateResponseDto();

                return Accepted(new { responseDto, resultLink, task.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Details = new List<string> { ex.Message } });
            }
        }


        /// <summary>
        /// Uploads an eligibility file for small companies and processes it.
        /// </summary>
        /// <param name="request">The eligibility file request details.</param>
        /// <returns>The complete report line by line.</returns>
        /// <response code="200">If the file is successfully processed, returns the results.</response>
        /// <response code="400">If the request is invalid or if an operation error occurs, returns a list of errors.</response>
        /// <response code="500">If an internal error occurs, returns an error message.</response>
        [HttpPost("small-companies")]
        [ProducesResponseType(typeof(EligibilityFileSmallCompaniesResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UploadFileSmallCompanies([FromBody] EligibilityFileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                return BadRequest(new ErrorResponse { Message = "Ups!", Details = errors });
            }
            try
            {
                var processingResult = await _eligibilityFileService.ProcessFileAsync(request.FileAddress, request.EmployerName, true);

                var responseDto = EligibilityResponseFactory.CreateResponseDto(processingResult);

                var resultLink = Url.Link("GetReportByEmployerName", new { employerName = request.EmployerName, pageNumber = 1, pageSize = 10 });

                return Ok(new { responseDto, resultLink });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = "Ops!", Details = new List<string>() { ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Details = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Retrieves reports based on the employer name with pagination.
        /// </summary>
        /// <param name="employerName">The name of the employer to filter the reports.</param>
        /// <param name="pageNumber">The current page number for pagination. Must be greater than 0.</param>
        /// <param name="pageSize">The number of items per page. Must be greater than 0.</param>
        /// <returns>Returns paginated reports or an error message if no reports are found or if an error occurs.</returns>
        /// <response code="200">Returns the paginated results of reports.</response>
        /// <response code="400">If the parameters are invalid, returns a message explaining the issue.</response>
        /// <response code="404">If no reports are found for the specified employer or page, returns a not found message.</response>
        /// <response code="500">If an internal error occurs, returns a generic error message.</response>
        [ProducesResponseType(typeof(PaginatedResult<EligibilityRecord>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [HttpGet("reports", Name = "GetReportByEmployerName")]
        public async Task<IActionResult> GetReportsByEmployerName([FromQuery] string employerName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new ErrorResponse { Details = new List<string>() { "PageNumber and PageSize must be greater than 0." } });
            }

            try
            {
                var paginatedResult = await _eligibilityFileService.GetEligibilityFileReport(employerName.ToUpper(), pageNumber, pageSize);

                if (paginatedResult == null || !paginatedResult.Items.Any())
                {
                    return NotFound($"No reports found for employer: {employerName}.");
                }

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Details = new List<string> { ex.Message } });
            }

        }
    }
}
