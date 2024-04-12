using Microsoft.AspNetCore.Mvc;
using OriginIAM.Api.Dtos.Request;
using OriginIAM.Api.Mappers;
using OriginIAM.Api.Models.Response;
using OriginIAM.Application.Interfaces;

namespace OriginIAM.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EligibilityFileController : ControllerBase
    {
        private readonly IEligibilityFileService _eligibilityFileService;

        public EligibilityFileController(IEligibilityFileService eligibilityFileService)
        {
            _eligibilityFileService = eligibilityFileService;
        }

        [HttpPost("")]
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

                return Accepted(new { responseDto, resultLink, task.Id});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Details = new List<string> { ex.Message } });
            }
        }


        [HttpPost("small-companies")]
        public async Task<IActionResult> UploadFileSmallCompanies([FromBody] EligibilityFileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                return BadRequest(new ErrorResponse { Message = "Ups!",  Details = errors });
            }
            try
            {
                var processingResult = await _eligibilityFileService.ProcessFileAsync(request.FileAddress, request.EmployerName, true);

                var responseDto = EligibilityResponseFactory.CreateResponseDto(processingResult);

                var resultLink = Url.Link("GetReportByEmployerName", new { employerName = request.EmployerName, pageNumber = 1, pageSize = 10 });

                return Ok(new { responseDto, resultLink } );
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
