using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Models;

namespace Domain.Interfaces.ITokenService
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(BaseUser baseUser);
    }
}
