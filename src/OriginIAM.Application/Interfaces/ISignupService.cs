using OriginIAM.Application.Dtos;
using OriginIAM.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Application.Interfaces
{
    public interface ISignupService
    {
        Task<SignupResult> SignupAsync(SignupDto request);
    }
}
