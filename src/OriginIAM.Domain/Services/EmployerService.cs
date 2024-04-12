using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;

        public EmployerService(IEmployerRepository employerRepository)
        {
            _employerRepository = employerRepository ?? throw new ArgumentNullException(nameof(employerRepository));
        }

        public Task<bool> AssociateUserWithEmployerAsync(string userId, string employerId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateEmployerAsync(Employer employer)
        {
            return await _employerRepository.AddAsync(employer);
        }

        public Task<bool> DisassociateUserFromEmployerAsync(string userId, string employerId)
        {
            throw new NotImplementedException();
        }

        public Task<Employer> GetEmployerByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetEmployerIdByName(string name)
        {
            return await _employerRepository.GetEmployerIdByNameAsync(name);
        }

        public async Task<bool> UpdateEmployerAsync(Employer employer)
        {
            return await _employerRepository.UpdateAsync(employer);
        }
    }
}
