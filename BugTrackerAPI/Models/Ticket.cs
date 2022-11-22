namespace BugTrackerAPI.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public string Title { get; set; }

        public string FullDescription { get; set; }

        public string Organization { get; set; }

        public string Project { get; set; }

        public string AppModule { get; set; }

        public string AppVersion { get; set; }

        public string ImageFilePath { get; set; }

        public int PriorityId { get; set; }

        public int StatusId { get; set; }

        public int AssignUserId { get; set; }

        public int CreateUserId { get; set; }

        public int UpdateUserId { get; set; }

        public string CreateDate { get; set; }

        public string UpdateDate { get; set; }
    }

    public class TicketView : Ticket
    {
        public string AssignUserName { get; set; }

        public string PriorityName { get; set; }

        public string StatusName { get; set; }

        public string UpdateUserName { get; set; }
    }

    public class TicketSummaryByUser
    {
        public int StatusId { get; set; }

        public string AssignUserName { get; set; }

        public string StatusName { get; set; }

        public int TicketCount  { get; set; }

        public string UserDescription { get; set; }

        public string PhotoFilePath { get; set; }
    }

}
