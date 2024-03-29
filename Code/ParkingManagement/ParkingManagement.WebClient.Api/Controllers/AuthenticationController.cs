﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Contract.Employee;
using ParkingManagement.WebClient.Api.Models.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.WebClient.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string tokenIssuer = "Parking.Authentication.Bearer";
        private const string tokenAudience = "Parking.Authentication.Bearer";
        private const int accessTokenExpiry = 5;
        private const int refreshTokenExpiry = 20;

        private readonly SecurityKey accessTokenKey;
        private readonly SecurityKey refreshTokenKey;

        private readonly IEmployeeSecurityManager employeeSecurityManager;

        public AuthenticationController(
            Framework.IConfig config,
            IEmployeeSecurityManager employeeSecurityManager)
        {
            this.employeeSecurityManager = employeeSecurityManager;

            accessTokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.AccessTokenKey));
            refreshTokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.RefreshTokenKey));
        }

        [HttpPost]
        public async Task<LoginResult> Login([FromBody] LoginModel loginModel)
        {
            Models.Authentication.LoginReturn loginReturn = await Task.Run(() =>
            {
                return employeeSecurityManager.Login(loginModel.Email, loginModel.Password).DeepCopy<Models.Authentication.LoginReturn>();
            });

            return GetLoginResult(loginReturn);
        }

        [HttpPost]
        public async Task<RegisterResult> Register([FromBody] RegisterModel registerModel)
        {
            RegisterReturn registerReturn = await Task.Run(() =>
            {
                return employeeSecurityManager.Register(new RegisterEmployee
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Email = registerModel.Email,
                    Password = registerModel.Password
                });
            });

            return GetRegisterResult(registerReturn);
        }

        [HttpPost]
        public RefreshReturn Refresh([FromBody] Token refreshToken)
        {
            ClaimsPrincipal employeeClaims = new JwtSecurityTokenHandler().ValidateToken(refreshToken.Value, new TokenValidationParameters()
            {
                ValidAudience = tokenAudience,
                ValidIssuer = tokenIssuer,
                IssuerSigningKey = refreshTokenKey,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            }, out SecurityToken validatedRefreshToken);

            Guid userID = new Guid(employeeClaims.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userID.ToString())
            };

            return new RefreshReturn
            {
                AccessToken = GenerateToken(claims, accessTokenExpiry, accessTokenKey),
                RefreshToken = GenerateToken(claims, refreshTokenExpiry, refreshTokenKey)
            };
        }

        private RegisterResult GetRegisterResult(RegisterReturn registerReturn)
        {
            RegisterResult registerResult = new()
            {
                Status = (Models.Authentication.RegisterStatus)registerReturn.Status
            };

            if (registerResult.Status != Models.Authentication.RegisterStatus.Success)
                return registerResult;

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, registerReturn.EmployeeID.ToString())
            };

            registerResult.AccessToken = GenerateToken(claims, accessTokenExpiry, accessTokenKey);
            registerResult.RefreshToken = GenerateToken(claims, refreshTokenExpiry, refreshTokenKey);

            return registerResult;
        }

        private LoginResult GetLoginResult(Models.Authentication.LoginReturn loginReturn)
        {
            LoginResult loginResult = new()
            {
                Status = loginReturn.Status
            };

            if (loginResult.Status != Models.Authentication.LoginStatus.Success)
                return loginResult;

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginReturn.EmployeeID.ToString())
            };

            loginResult.AccessToken = GenerateToken(claims, accessTokenExpiry, accessTokenKey);
            loginResult.RefreshToken = GenerateToken(claims, refreshTokenExpiry, refreshTokenKey);

            return loginResult;
        }

        private static Token GenerateToken(Claim[] claims, double expiryTime, SecurityKey securityKey)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = tokenIssuer,
                Audience = tokenAudience,
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.UtcNow.AddMinutes(expiryTime),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return new Token
            {
                Expires = token.ValidTo,
                Value = tokenHandler.WriteToken(token)
            };
        }
    }
}
