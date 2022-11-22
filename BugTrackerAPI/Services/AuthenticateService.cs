using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using BugTrackerAPI.Models;

namespace BugTrackerAPI.Services
{
    public interface IAuthenticateService
    {
        string GenerateToken();

        TokenResponse GenerateTokenResponse(User user, string token);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public AuthenticateService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        // public string GenerateToken(User _user)
        public string GenerateToken()
        {
            //create claims details based on the user information
            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                    //new Claim(ClaimTypes.Sid, _user.UserId.ToString()),
                    //new Claim(ClaimTypes.Role, _user.UserDescription),
                    //new Claim(ClaimTypes.Name, _user.UserName),
                    //new Claim(ClaimTypes.Email, _user.EmailAddress)
                   };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

            return (new JwtSecurityTokenHandler().WriteToken(token));
        }

        public TokenResponse GenerateTokenResponse(User _user, string _token)
        {
            TokenResponse _TokenResponse = new TokenResponse();

            if ((_user != null) && (_user.UserId > 0))
            {
                _TokenResponse.Success = true;
                _TokenResponse.AccessToken = _token;
                _TokenResponse.UserId = _user.UserId;
                _TokenResponse.UserName = _user.UserName;
                _TokenResponse.EmailAddress = _user.EmailAddress;
                _TokenResponse.PhotoFilePath = _user.PhotoFilePath;
            }
            else
            {
                _TokenResponse.Success = false;
                _TokenResponse.Error = "Invalid credentials";
                _TokenResponse.ErrorCode = "invalid_grant";
            }
            
            return _TokenResponse;
        }
    }
}
