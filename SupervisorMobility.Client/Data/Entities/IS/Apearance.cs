
namespace SupervisorMobility.Client.Data.Entities.IS
{
    public class Apearance
    {
        public int ApearanceId { get; set; }
        public bool? IsActive { get; set; }
        
        public int? PartId { get; set; }
        public Part? Part { get; set; }

        public ICollection<Commentary>? Observations { get; set; }

        public int? ManufacturerId { get; set; }
        public User? Manufacturer { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverUserId { get; set; }
        public User? ApproverUser { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? CheckDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        //Item de la categoria
        public ICollection<DataPanelSpecification>? DataPanelSpecificationItems { get; set; }
         = new List<DataPanelSpecification>();

        //Item de los problemas
        public ICollection<ProblemDefect>? ProblemDefectItems { get; set; }
         = new List<ProblemDefect>();

        public ICollection<LogbookAparence>? LogbooksAparence { get; set; }
         = new List<LogbookAparence>();

    }
}
