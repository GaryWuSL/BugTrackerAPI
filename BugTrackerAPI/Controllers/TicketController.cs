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
    public class TicketController : Controller
    {
        private readonly ITicketService _ts;

        public TicketController(ITicketService ticketservice)
        {
            _ts = ticketservice;
        }

        [HttpGet]
        public JsonResult Get()
        {
            Ticket _ticket = new Ticket();
            List<TicketView> _lstticketviews = new List<TicketView>();
            _lstticketviews = _ts.GetTicketViews(_ticket);
            return new JsonResult(_lstticketviews);
        }

        [HttpGet("/api/[controller]/get")]
        public JsonResult Get([FromQuery] int ticketid)
        {
            TicketView _ticketview = new TicketView();
            _ticketview = _ts.GetTicketView(ticketid);
            return new JsonResult(_ticketview);
        }

        [HttpGet("/api/[controller]/summarybyuser")]
        public IActionResult SummaryByUser()
        {
            List<TicketSummaryByUser> _ticketsumbyuser = new List<TicketSummaryByUser>();
            _ticketsumbyuser = _ts.GetTicketSummarybyUser();
            return Ok(_ticketsumbyuser);
        }

        [HttpPost]
        public JsonResult Post(Ticket _ticket)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _ts.New(_ticket);
            return new JsonResult(_sr);
        }


        [HttpPut]
        public JsonResult Put(Ticket _ticket)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _ts.Set(_ticket);
            return new JsonResult(_sr);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int _ticketid)
        {
            ServiceResponse _sr = new ServiceResponse();
            _sr = _ts.Del(_ticketid);
            return new JsonResult(_sr);
        }
    }
}
