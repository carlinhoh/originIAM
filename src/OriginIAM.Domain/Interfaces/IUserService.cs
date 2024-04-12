using OriginIAM.Domain.Entities;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<string> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> PatchUserDetailsAsync(string email, string user, decimal? salary);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> SetUserAsPendingByEmployerId(string employerId);
        Task<bool> TerminateOldAccounts(string employerId);

    }
}
