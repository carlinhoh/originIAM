using OriginIAM.Domain.Entities;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Interfaces
{
    public interface IEmployerRepository
    {
        Task<Employer> GetEmployerByNameAsync(string employerName);
        Task<bool> AddAsync(Employer employer);
        Task<bool> UpdateAsync(Employer employer);
        Task<bool> DeleteAsync(string employerName);
        Task<string> GetEmployerIdByNameAsync(string employerName);

    }
}
