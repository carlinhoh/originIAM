using OriginIAM.Domain.Entities;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Interfaces
{
    public interface IEmployerService
    {
        Task<Employer> GetEmployerByNameAsync(string name);
        Task<string> GetEmployerIdByName(string name); 
        Task<bool> CreateEmployerAsync(Employer employer);
        Task<bool> UpdateEmployerAsync(Employer employer);
        Task<bool> AssociateUserWithEmployerAsync(string userId, string employerId);
        Task<bool> DisassociateUserFromEmployerAsync(string userId, string employerId);
    }
}
