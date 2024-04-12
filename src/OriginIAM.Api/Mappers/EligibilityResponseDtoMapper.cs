using OriginIAM.Api.Dtos.Response;
using OriginIAM.Application.Services;

namespace OriginIAM.Api.Mappers
{
    public static class EligibilityResponseFactory
    {
        public static EligibilityBaseFileResponseDto CreateResponseDto(EligibilityProcessingResult result)
        {
            if (result is EligibilityProcessingSmallCompaniesResult smallCompanyResult)
            {
                return new EligibilityFileSmallCompaniesResponseDto
                {
                    Message = "File processed successfully.",
                    TotalProcessed = smallCompanyResult.TotalProcessed,
                    TotalFailed = smallCompanyResult.TotalFailed,
                    ProcessedRecords = smallCompanyResult.SuccessfulRecords,
                    FailedRecords = smallCompanyResult.FailedRecords
                };
            }
            else
            {
                return new EligibilityFileResponseDto
                {
                    Message = "Your file is being processed."
                };
            }
        }

        public static EligibilityBaseFileResponseDto CreateResponseDto()
        {
            return new EligibilityFileResponseDto
            {
                Message = "Your file is being processed."
            };
        }
    }
}
