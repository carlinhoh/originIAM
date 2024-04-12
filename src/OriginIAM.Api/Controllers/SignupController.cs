using Microsoft.AspNetCore.Mvc;
using OriginIAM.Api.Models.Response;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Dtos;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OriginIAM.Application.Models;
using OriginIAM.Api.Dtos.Request;
using OriginIAM.Api.Mappers.OriginIAM.Api.Mappers;

namespace OriginIAM.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignupController : ControllerBase
    {
        private readonly ISignupService _signupService;

        public SignupController(ISignupService signupService)
        {
            _signupService = signupService ?? throw new ArgumentNullException(nameof(signupService));
        }

        [HttpPost("")]
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
                    return CreatedAtAction(nameof(GetUser), new { id = signupResult.UserId }, signupResult);
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

        private SignupDto MapRequestToDto(SignupRequestDto request)
        {
            return new SignupDto
            {
                Email = request.Email,
                Password = request.Password,
                Country = request.Country,
                FullName = request.FullName,
                AcceptTerms = request.AcceptTerms,
                BirthDate = request.BirthDate
            };
        }

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
