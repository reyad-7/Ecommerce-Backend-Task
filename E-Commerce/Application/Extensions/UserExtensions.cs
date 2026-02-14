using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOS.Auth;
using Domain.Entities.Models;

namespace Application.Extensions
{
    public static class UserExtensions
    {
        public static AuthResponseDto 
            ToAuthResponseDto(this BaseUser user, string token, int DurationInDays)
        {
            return new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiresAt = DurationInDays
            };
        }
    }
}
