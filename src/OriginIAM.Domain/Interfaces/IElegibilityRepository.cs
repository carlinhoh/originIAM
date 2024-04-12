using OriginIAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Interfaces
{
    public interface IEligibilityRepository
    {
        Task SaveOrUpdateEligibleUser(User user);
        Task<User> GetAndDeleteUser(string email, string password);
        Task<bool> DeleteUser(string email);
    }
}
