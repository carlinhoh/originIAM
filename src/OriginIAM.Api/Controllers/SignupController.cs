using Microsoft.AspNetCore.Mvc;
using OriginIAM.Api.Models.Response;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Models;
using OriginIAM.Api.Dtos.Request;
using OriginIAM.Api.Mappers.OriginIAM.Api.Mappers;

namespace OriginIAM.Api.Controllers
{
    /// <summary>
    /// Controller for managing user signups.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SignupController : ControllerBase
    {
        private readonly ISignupService _signupService;

        public SignupController(ISignupService signupService)
        {
            _signupService = signupService ?? throw new ArgumentNullException(nameof(signupService));
        }

        // <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="request">The signup details.</param>
        /// <returns>A response indicating whether the signup was successful.</returns>
        /// <response code="201">Returns the newly created user details.</response>
        /// <response code="400">If the request is invalid, returns a list of errors.</response>
        /// <response code="500">If an internal error occurs, returns an error message.</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(SignupResult), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                return BadRequest(new ErrorResponse { Details = errors });
            }

            try
            {
                var signupDto = SignupDtoMapper.ToSignupDto(request);

                var signupResult = await _signupService.SignupAsync(signupDto);

                if (signupResult.Success)
                {
                    return CreatedAtAction(nameof(GetUser), new { id = signupResult.UserId}, signupResult);
                }
                else
                {
                    return BadRequest(signupResult);
                }
            }
            catch (Exception ex)
            {
                //TODO: Log
                return StatusCode(500, new ErrorResponse { Details = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Retrieves a user by their unique identifier. For simplicity's sake, this will not be implemented.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user details if found; otherwise, NotFound.</returns>
        [ProducesResponseType(typeof(SignupResult), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            //Assuming that this method was implemented in the Users api, the creation would be returned pointing to this get.
            var user = ""; 

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
