using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        //Supervisor
        public int? SuperiorId { get; set; }
        public User? Superior { get; set; }
        //Info de Usuario
        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? CurrentdistributionId { get; set; }
        public Distribution? currentdistribution { get; set; }

        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
