using OriginIAM.Api.Dtos.Request;
using OriginIAM.Application.Dtos;

namespace OriginIAM.Api.Mappers
{
    namespace OriginIAM.Api.Mappers
    {
        public static class SignupDtoMapper
        {
            public static SignupDto ToSignupDto(SignupRequestDto requestDto)
            {
                if (requestDto == null) throw new ArgumentNullException(nameof(requestDto));

                return new SignupDto
                {
                    Email = requestDto.Email,
                    Password = requestDto.Password,
                    Country = requestDto.Country,
                    FullName = requestDto.FullName,
                    AcceptTerms = requestDto.AcceptTerms,
                    BirthDate = requestDto.BirthDate
                };
            }
        }
    }
}
