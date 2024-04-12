using OriginIAM.Application.Dtos;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Models;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;

namespace OriginIAM.Application.Services
{
    public class SignupService : ISignupService
    {
        private readonly IEligibilityService _eligibilityService;
        private readonly IUserService _userService;

        public SignupService(IEligibilityService eligibilityService, IUserService userService)
        {
            _eligibilityService = eligibilityService;
            _userService = userService;
        }

        public async Task<SignupResult> SignupAsync(SignupDto request)
        {
            //1. Check if the email is associated with some employer via the eligibility file
            var user = await _eligibilityService.GetAndDeleteUser(request.Email, request.Password);
            
            if (user != null) {
                //i. If it is, use employer-provided data to create the user
                var userFromEmployerData = await _userService.CreateUserAsync(user);

                //Removing user from cache
                await _eligibilityService.DeleteUser(request.Email);

                return SingupSucessfullyCreated(userFromEmployerData);
            }

            // ii. If not, validate if the email already exists
            user = await _userService.GetUserByEmailAsync(request.Email);

            if (user != null)
            {
                //User already exists
                return userAlreadyExists(user.Email);
            }

            //2. Validate password strength - Already done in the api Model. 

            //3. Create the user on User Service
            var userId = await _userService.CreateUserAsync(SignupDtoToUser(request));

            return SingupSucessfullyCreated(userId);

        }

        #region private methods

        private SignupResult SingupSucessfullyCreated(string userId)
        {
            return new SignupResult()
            {
                UserId = userId,
                Success = true,
                Message = "User created successfully! Enjoy the platform."
            };
        }

        private SignupResult userAlreadyExists(string email)
        {
            return new SignupResult()
            {
                UserId = email,
                Success = true,
                Message = "User already created."
            };
        }

        private User SignupDtoToUser(SignupDto signupDto)
        {
            return new User(signupDto.Email, signupDto.Password, signupDto.Country, string.Empty, signupDto.FullName);
        }
        #endregion
    }
}
