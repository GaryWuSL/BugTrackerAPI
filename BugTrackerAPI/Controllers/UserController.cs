using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BugTrackerAPI.Models;
using BugTrackerAPI.Services;

namespace BugTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _us;
        public UserController(IUserService userservice)
        {
            _us = userservice;
        }

        public User Get(string _username, string _password)
        {
            User _user = new User();
            _user = _us.Get(_username, _password);
            return _user;
        }

        [HttpGet]
        public JsonResult Get()
        {
            User _user = new User();
            List<User> _lstuser = new List<User>();
            _lstuser = _us.GetUsers(_user);
            return new JsonResult(_lstuser);
        }


        [HttpPost]
        public JsonResult Post(User _user)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _us.New(_user);
            return new JsonResult(_sr);
        }


        [HttpPut]
        public JsonResult Put(User _user)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _us.Set(_user);
            return new JsonResult(_sr);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int _id)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _us.Del(_id);
            return new JsonResult(_sr);
        }
    }
}
