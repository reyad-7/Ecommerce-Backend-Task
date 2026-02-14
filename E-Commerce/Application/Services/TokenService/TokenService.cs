using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Models;
using Domain.Interfaces.ITokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.TokenService
{
    public class TokenService : ITokenService
        {
            private readonly UserManager<BaseUser> _userManager;
            private readonly string _securityKey;
            private readonly string _issuer;
            private readonly string _audience;
            private readonly int _durationInDays;


            public TokenService(UserManager<BaseUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _securityKey = configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is not configured.");
                _issuer = configuration["Jwt:Issuer"] ?? "WaffarXEcommerce";
                _audience = configuration["Jwt:Audience"] ?? "WaffarXEcommerceUsers";
                _durationInDays = int.Parse(configuration["Jwt:DurationInDays"] ?? "90");
            }

            public async Task<string> CreateTokenAsync(BaseUser baseUser)
            {
                var userRoles = await _userManager.GetRolesAsync(baseUser);
                var role = userRoles.FirstOrDefault() ?? "Customer"; 

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, baseUser.Id),
                new Claim(ClaimTypes.Name, baseUser.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, baseUser.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(_durationInDays),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
    }
}
