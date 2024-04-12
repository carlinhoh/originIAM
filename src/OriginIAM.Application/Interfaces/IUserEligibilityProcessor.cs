using OriginIAM.Application.Models;

namespace OriginIAM.Application.Services
{
    public interface IUserEligibilityProcessor
    {
        Task ProcessUserEligibilityAsync(EligibilityRecord eligibilityRecord, string employerId);
        Task<string> GetEmployerIdByName(string employerName);
        Task<bool> SetUserAsPendingConfirmation(string employerId);
        Task<bool> TerminateOldAccounts(string employerId);
    }
}