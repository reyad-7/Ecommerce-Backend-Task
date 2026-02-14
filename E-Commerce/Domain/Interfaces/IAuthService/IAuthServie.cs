using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Auth;
using Domain.Entities.GeneralResponse;

namespace Domain.Interfaces.IAuthService
{
    public interface IAuthServie
    {
        public Task< GeneralResponse.GeneralResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        public Task<GeneralResponse.GeneralResponseDto <AuthResponseDto>> LoginAsync(LoginDto loginDto);
    }
}
