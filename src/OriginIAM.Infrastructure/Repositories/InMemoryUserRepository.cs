using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.Metrics;

namespace OriginIAM.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        public Dictionary<string, User> _users = new Dictionary<string, User>();

        public InMemoryUserRepository()
        {

            _users = new Dictionary<string, User>
            {
                { "daniel93@dijkstra.com", new User("daniel93@dijkstra.com", "hashedpassword1", "US", "1") },
                { "derekcooper@dijkstra.com", new User("derekcooper@dijkstra.com", "hashedpassword2", "UK", "1") },
                { "removed_0@dijkstra.com", new User("removed_0@dijkstra.com", "hashedpassword3", "BR", "1") },
                { "removed_1@dijkstra.com", new User("removed_1@dijkstra.com", "hashedpassword4", "US", "1") },
                { "removed_2@dijkstra.com", new User("removed_2@dijkstra.com", "hashedpassword5", "ES", "1") }
            };
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            return Task.FromResult(_users.Values.FirstOrDefault(u => u.Email == email));
        }

        public Task<string> AddAsync(User user)
        {
            var currentUser = new User(user.Email, user.PasswordHash, user.Country, string.Empty);
            _users[currentUser.Id] = user;
            return Task.FromResult(currentUser.Id);
        }

        public Task<bool> UpdateAsync(User user)
        {
            _users[user.Id] = user;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string userEmail)
        {
            return Task.FromResult(_users.Remove(userEmail));
        }

        public Task<bool> TerminateUsersByEmployerIdAsync(string employerId)
        {
            var keysToRemove = _users.Where(kvp => kvp.Value.EmployerId == employerId && !kvp.Value.VerifiedEmployee)
                                     .Select(kvp => kvp.Key)
                                     .ToList();

            foreach (var key in keysToRemove)
            {
                _users.Remove(key);
            }

            return Task.FromResult(true);
        }

        public Task<bool> SetUserAsPendingByEmployerId(string employerId)
        {
            var usersToSetAsPending = _users.Values
                                            .Where(user => user.EmployerId == employerId && user.IsActive && user.VerifiedEmployee)
                                            .ToList();

            foreach (var user in usersToSetAsPending)
            {
                user.VerifiedEmployee = false;
            }

            return Task.FromResult(true);   
        }

        public Task<bool> PatchUserDetailsAsync(string email, string country, decimal? salary)
        {
            if (_users.TryGetValue(email, out var user))
            {
                user.Country = country;
                user.Salary = salary;

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
