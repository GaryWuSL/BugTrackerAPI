using System;

namespace BugTrackerAPI.Models
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        
        public string ErrorCode { get; set; }
        
        public string Error { get; set; }
    }

    public class TokenResponse : BaseResponse
    {
        public string AccessToken { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PhotoFilePath { get; set; }

        public TokenResponse()
        {
            Success = false;
            Error = string.Empty;
            ErrorCode = string.Empty;
            AccessToken = string.Empty;
            UserId = 0;
            UserName = string.Empty;
            EmailAddress = string.Empty;
            PhotoFilePath = string.Empty;
        }
    }

    public class ServiceResponse : BaseResponse
    {
        public int RecordCount { get; set; }

        public ServiceResponse() {
            Success = false;
            Error = string.Empty;
            ErrorCode = string.Empty;
            RecordCount = 0;
        }
    }

}
