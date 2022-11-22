using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BugTrackerAPI.Services;

namespace BugTrackerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : Controller
    {
        private readonly IShareService _ss;

        public AttachmentController(IShareService shareService)
        {
            _ss = shareService;
        }

        [HttpPost]
        [Route("SaveFile")]
        public async Task<JsonResult> SaveFile()
        {
            string strFileName = await UploadFile();

            return new JsonResult(strFileName);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get(string filename)
        {
            FileStream imageFile = _ss.GetFile(filename);
            return File(imageFile, "image/jpeg");
        }

        private Task<string> UploadFile()
        {
            return Task.Run(() =>
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = _ss.UploadFile(postedFile);

                return filename;
            });
        }
    }
}
