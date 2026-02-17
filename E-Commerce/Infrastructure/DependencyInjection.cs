using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.AuthService;
using Application.Services.CategoryService;
using Application.Services.OrderService;
using Application.Services.ProductService;
using Application.Services.RedisCacheService;
using Application.Services.TokenService;
using Domain.Entities.Models;
using Domain.Interfaces.ICacheService;
using Domain.Interfaces.ICategoryService;
using Domain.Interfaces.IOrderService;
using Domain.Interfaces.IProductService;
using Domain.Interfaces.ITokenService;
using Domain.Interfaces.IunitOfWork;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // db context
            services.AddDbContext<WaffarXEcommerceDBContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = configuration["Redis:InstanceName"];
            });

            //  Cache Service
            var defaultTTL = int.Parse(configuration["Redis:DefaultTTLMinutes"] ?? "30");
            services.AddSingleton<ICacheService>(provider =>
            {
                var cache = provider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                return new RedisCacheService(cache, defaultTTL);
            });

            // Identity
            services.AddIdentity<BaseUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<WaffarXEcommerceDBContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            var jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<Domain.Interfaces.IAuthService.IAuthServie, AuthService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
