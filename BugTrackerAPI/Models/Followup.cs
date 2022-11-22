using System;

namespace BugTrackerAPI.Models
{
    public class Followup
    {
        public int FollowupId { get; set; }

        public int TicketId { get; set; }

        public string Title { get; set; }

        public string FullDescription { get; set; }

        public int PriorityId { get; set; }

        public int StatusId { get; set; }

        public int AssignUserId { get; set; }

        public int CreateUserId { get; set; }

        public int UpdateUserId { get; set; }

        public string CreateDate { get; set; }

        public string UpdateDate { get; set; }
    }

    public class FollowupView : Followup
    {
        public string AssignUserName { get; set; }

        public string PriorityName { get; set; }

        public string StatusName { get; set; }

        public string UpdateUserName { get; set; }
    }
}
