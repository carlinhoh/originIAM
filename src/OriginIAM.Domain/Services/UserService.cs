using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<string> CreateUserAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> PatchUserDetailsAsync(string email, string country, decimal? salary)
        {
            return await _userRepository.PatchUserDetailsAsync(email, country, salary);
        }

        public Task<bool> DeactivateUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TerminateOldAccounts(string employerId)
        {
            return _userRepository.TerminateUsersByEmployerIdAsync(employerId); 
        }

        public async Task<bool> SetUserAsPendingByEmployerId(string employerId)
        {
           return await _userRepository.SetUserAsPendingByEmployerId(employerId);
        }
    }
}
