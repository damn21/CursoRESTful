using Application.DTOs.Users;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Settings;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Identity.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly JWTSettings jWTSettings;
        private readonly IDateTimeService dateTimeService;
        public AccountServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<JWTSettings> jWTSettings, IDateTimeService dateTimeService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.jWTSettings = jWTSettings.Value;
            this.dateTimeService = dateTimeService;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticationAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ApiException($"No existe una cuenta registrada con el email ${request.Email}");
            }

            var result = await signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ApiException($"Las credenciales del usuario no son válidas ${request.Email}");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id;
            response.UserName = user.UserName;
            response.Email = user.Email;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;

            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return new Response<AuthenticationResponse>(response, $"Usuario autenticado {user.UserName}");

        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userName = await userManager.FindByNameAsync(request.UserName);

            var userEmail = await userManager.FindByEmailAsync(request.Email);

            if (userName != null)
            {
                throw new ApiException($"El usuario de nombre {request.UserName} se encuentra registrado");
            }

            if (userEmail != null)
            {
                throw new ApiException($"El usuario con email {request.Email} se encuentra registrado");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var client = await userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                return new Response<string>(user.Id, message: $"Usuario registrado exitosamente");
            }
            else
            {
                throw new ApiException($"{result.Errors}");
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roleClaims.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var Securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSettings.Key));
            var creds = new SigningCredentials(Securitykey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddMinutes(jWTSettings.DurationInMinutes);

            var securityToken = new JwtSecurityToken(
                issuer: jWTSettings.Issuer,
                audience: jWTSettings.Audience, 
                claims: claims,
                expires: expiration, 
                signingCredentials: creds
                );

            return securityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreatedByIp = ipAddress
            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoService = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoService.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-","");
        }
    }
}
