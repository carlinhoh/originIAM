using Newtonsoft.Json;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Services
{
    public class EligibilityService : IEligibilityService
    {
        private readonly IEligibilityRepository _eligibilityRepository;
        public EligibilityService(IEligibilityRepository eligibilityRepository)
        {
            _eligibilityRepository = eligibilityRepository;
        }

        public async Task<bool> DeleteUser(string email)
        {
            return await _eligibilityRepository.DeleteUser(email);
        }

        public async Task<User> GetAndDeleteUser(string email, string password)
        {
            return await _eligibilityRepository.GetAndDeleteUser(email, password);
        }

        public async Task SaveOrUpdateEligibleUser(User user)
        {
            await _eligibilityRepository.SaveOrUpdateEligibleUser(user);
        }
    }

}
