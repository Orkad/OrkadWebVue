﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrkadWeb.Logic.Users.Commands;

namespace OrkadWeb.Angular.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public AuthenticationController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<LoginResponse> Login(LoginCommand command)
        {
            var response = await mediator.Send(command, HttpContext.RequestAborted);
            if (response.Success)
            {
                response.Token = GenerateJSONWebToken(response);  
            }
            return response;
        }

        private string GenerateJSONWebToken(LoginResponse loginResponse)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], GetClaims(loginResponse), expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Claim[] GetClaims(LoginResponse loginResponse)
        {
            return new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ConvertToUnixTimestamp(DateTime.Now).ToString()),
                new Claim("user_id", loginResponse.Id),
                new Claim("user_name", loginResponse.Name),
                new Claim("user_email", loginResponse.Email),
            };
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}