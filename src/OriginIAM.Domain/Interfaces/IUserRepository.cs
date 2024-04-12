using OriginIAM.Domain.Entities;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<string> AddAsync(User user);
        Task<bool> UpdateAsync(User user);

        Task<bool> PatchUserDetailsAsync(string id, string country, decimal? salary);
        Task<bool> DeleteAsync(string userEmail);
        Task<bool> TerminateUsersByEmployerIdAsync(string employerId);
        Task<bool> SetUserAsPendingByEmployerId(string employerId);
    }
}
