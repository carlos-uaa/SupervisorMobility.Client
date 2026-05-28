

namespace SupervisorMobility.Clinet.Data.Entities.Dtos.HRIHistory
{
    public class GetHRIHistoryActionDto
    {
        public int HistoryId { get; set; }
        public int HRIid { get; set; }
        public int ResponsibleUserId { get; set; }
        public GetUserForHRIDailyRevsionDto Responsible { get; set; }
        public string Action { get; set; }
        public string ActionType { get; set; } = "UPDATE";
        public DateTime ActionDate { get; set; }
    }
}
