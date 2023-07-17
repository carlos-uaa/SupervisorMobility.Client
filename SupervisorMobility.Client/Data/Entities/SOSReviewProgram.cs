using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities
{
    public class SOSReviewProgram
    {
        public int SOSid { get; set; }
        public int Status { get; set; }

        public int? UserAid { get; set; }
        public User? UserA { get; set; }

        public int? UserBid { get; set; }
        public User? UserB { get; set; }

        public int? UserCid { get; set; }
        public User? UserC { get; set; }


        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

        public int? AreaId { get; set; }

        public Area? Area { get; set; }

        public DateTime? CreationDate { get; set; }
        public int? AplicationYear
        {
            get { return CreationDate?.Year; }
            set
            {
                if (value != null)
                {
                    int year = value.Value;
                    if (!(year >= 1 && year <= 9999))
                    {

                        AplicationYear = CreationDate?.Year;
                    }
                }
                else
                {
                    AplicationYear = null;
                }
            }
        }


        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public bool IsActive { get; set; }
    }
}
