using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OriginIAM.Infrastructure.Repositories
{
    public class InMemoryEmployerRepository : IEmployerRepository
    {
        private readonly Dictionary<string, Employer> _employers = new Dictionary<string, Employer>();

        public InMemoryEmployerRepository()
        {
            _employers = new Dictionary<string, Employer>
            {
                { "dijkstra", new Employer("Dijkstra", "1") },
                { "knuth", new Employer("Knuth", "2") },
                { "hoare", new Employer("Hoare", "3") },
                { "liskov", new Employer("Liskov", "4") },
                { "perlman", new Employer("Perlman", "5") },


            };
        }

        public Task<Employer> GetEmployerByNameAsync(string employerName)
        {
            return Task.FromResult(_employers.Values.FirstOrDefault(e => e.Name == employerName));
        }

        public Task<string> GetEmployerIdByNameAsync(string employerName)
        {
            if (_employers.ContainsKey(employerName)){
                return Task.FromResult(_employers[employerName].Name);
            }

            return Task.FromResult(string.Empty);
            
        }
        public Task<bool> AddAsync(Employer employer)
        {
            _employers[employer.Id] = employer;
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(Employer employer)
        {
            _employers[employer.Id] = employer;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string employerName)
        {
            return Task.FromResult(_employers.Remove(employerName));
        }
    }
}
