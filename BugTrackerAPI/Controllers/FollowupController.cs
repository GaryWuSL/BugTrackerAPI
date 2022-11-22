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
    public class FollowupController : Controller
    {
        private readonly ITicketService _ts;
        private readonly IFollowupService _fs;

        public FollowupController(ITicketService ticketservice, IFollowupService followupservice)
        {
            _ts = ticketservice;
            _fs = followupservice;
        }

        [HttpGet("/api/[controller]/get")]
        public JsonResult Get([FromQuery] int ticketId)
        {
            List<FollowupView> _listfollowupview = new List<FollowupView>();
            _listfollowupview = _fs.GetFollowupViews(ticketId);
            return new JsonResult(_listfollowupview);
        }

        [HttpPost]
        public JsonResult Post(Followup _followup)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _fs.New(_followup);
            return new JsonResult(_sr);
        }


        [HttpPut]
        public JsonResult Put(Followup _followup)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _fs.Set(_followup);
            return new JsonResult(_sr);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int _followupid)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _fs.Del(_followupid);
            return new JsonResult(_sr);
        }
    }
}
