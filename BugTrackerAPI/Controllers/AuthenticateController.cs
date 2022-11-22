using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTrackerAPI.Models;
using BugTrackerAPI.Services;

namespace BugTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserService _us;
        private readonly IAuthenticateService _as;

        public AuthenticateController(IUserService userservice, IAuthenticateService authenticateservice)
        {
            _us = userservice;
            _as = authenticateservice;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<TokenResponse> Post(User user)
        {
            TokenResponse _tokenResponse = new TokenResponse();
            string strToken = string.Empty;

            if (user != null && (!(string.IsNullOrWhiteSpace(user.UserName))) && (!(string.IsNullOrWhiteSpace(user.Password))))
            {
                var objUser = await this.GetUser(user.UserName, user.Password);

                if ((objUser != null) && (objUser.UserId > 0))
                {
                    strToken = _as.GenerateToken();

                    if (!(string.IsNullOrWhiteSpace(strToken)))
                    {
                        _tokenResponse = _as.GenerateTokenResponse(objUser, strToken);
                    } else
                    {
                        _tokenResponse.Success = false;
                        _tokenResponse.Error = "Failure to generate authenticate token";
                        _tokenResponse.ErrorCode = "invalid_grant";
                    }
                }
                else
                {
                    _tokenResponse.Success = false;
                    _tokenResponse.Error = "Invalid credentials";
                    _tokenResponse.ErrorCode = "invalid_grant";
                }
            }
            return _tokenResponse;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logout")]
        public TokenResponse Logout()
        {
            return (new TokenResponse());
        }

        private Task<User> GetUser(string _userName, string _password)
        {
            return Task.Run(() =>
            {
                return _us.Get(_userName, _password);
            });
        }
    }
}
