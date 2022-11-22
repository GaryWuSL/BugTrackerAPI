using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerAPI.Models
{
    public class UserLogin
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
    public class User : UserLogin
    {
        public int UserId { get; set; }

        public string UserDescription { get; set; }

        public string ConfirmPassword { get; set; }

        public string EmailAddress { get; set; }

        public string PhotoFilePath { get; set; }

        public User()
        {
            UserId = 0;
            UserName = string.Empty;
            UserDescription = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            PhotoFilePath = string.Empty;
            EmailAddress = string.Empty;
        }

    }
}
