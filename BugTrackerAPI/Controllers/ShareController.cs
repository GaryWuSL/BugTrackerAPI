using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BugTrackerAPI.Models;
using BugTrackerAPI.Services;

namespace BugTrackerAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class ShareController : Controller
    {
        private readonly IShareService _shareservice;
        private readonly ISetupService _setupservice;
        public ShareController(IShareService shareservice, ISetupService setupservice)
        {
            _shareservice = shareservice;
            _setupservice = setupservice;
        }

        [HttpGet]
        [Route("api/[controller]/status")]
        
        public JsonResult Status()
        {
            List<Status> _lststatus = new List<Status>();
            _lststatus = _shareservice.GetStatusList();
            return new JsonResult(_lststatus);
        }

        [HttpGet]
        [Route("api/[controller]/priority")]

        public JsonResult Priority()
        {
            List<Priority> _lstpriority = new List<Priority>();
            _lstpriority = _shareservice.GetPriorityList();
            return new JsonResult(_lstpriority);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/[controller]/createdb")]
        public JsonResult CreateDB()
        {
            ServiceResponse _serviceResponse = new ServiceResponse();
            _serviceResponse = _setupservice.CreateDB();
            return new JsonResult(_serviceResponse);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/[controller]/importdata")]

        public JsonResult ImportData()
        {
            ServiceResponse _serviceResponse = new ServiceResponse();
            _serviceResponse = _setupservice.ImportData();
            return new JsonResult(_serviceResponse);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/[controller]/dbname")]
        public JsonResult DBName()
        {
            return new JsonResult(_setupservice.DBName());
        }
    }
}
