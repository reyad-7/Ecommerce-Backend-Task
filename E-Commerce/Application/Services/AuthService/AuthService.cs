using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Extensions;
using Domain.DTOS.Auth;
using Domain.Entities.GeneralResponse;
using Domain.Entities.Models;
using Domain.Interfaces.IAuthService;
using Domain.Interfaces.ITokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace Application.Services.AuthService
{
    public class AuthService : IAuthServie
    {
        private readonly UserManager<BaseUser> _userManager;
        private readonly SignInManager<BaseUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly int _durationInDays;
        public AuthService(UserManager<BaseUser> userManager, SignInManager<BaseUser> signInManager, ITokenService tokenservuce, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenservuce;
            _durationInDays = int.Parse(configuration["Jwt:DurationInDays"] ?? "90");
        }
        public async Task<GeneralResponse.GeneralResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // check if the username already exists
                var exsitingUsername = await _userManager.FindByNameAsync(registerDto.UserName);
                if (exsitingUsername is not null)
                {
                    return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse("Username already exists");
                }
                // check if the email already exists
                var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingEmail is not null)
                {
                    return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse("Email already exists");
                }
                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse("Passwords do not match");
                }
                // create a new user
                var usertoRegister = new BaseUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    EmailConfirmed = true,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                };
                var result = await _userManager.CreateAsync(usertoRegister, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GeneralResponseDto<AuthResponseDto>.FailureResponse(
                        $"Registration failed: {errors}"
                    );
                }
                // assign the user to the "Customer" role
                await _userManager.AddToRoleAsync(usertoRegister, "Customer");
                // generate a token for the user by my Token Serivce
                var token = await _tokenService.CreateTokenAsync(usertoRegister);
                var response = usertoRegister.ToAuthResponseDto(token, _durationInDays);
                return GeneralResponse.GeneralResponseDto<AuthResponseDto>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse(ex.Message);
            }
            return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse("An error occurred during registration");

        }
        public async Task<GeneralResponse.GeneralResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user is null)
                {
                    return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse("Invalid email or password");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return GeneralResponse.GeneralResponseDto<AuthResponseDto>.FailureResponse("Invalid email or password");
                }
                var token = await _tokenService.CreateTokenAsync(user);
                var response = user.ToAuthResponseDto(token, _durationInDays);
                return GeneralResponse.GeneralResponseDto<AuthResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                return GeneralResponseDto<AuthResponseDto>.FailureResponse(
                    $"Error during login: {ex.Message}"
                );
            }
        }
    }
}
