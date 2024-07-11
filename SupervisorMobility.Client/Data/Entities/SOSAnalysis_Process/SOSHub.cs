
namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class SOSHub
    {
        public int SOSHubId { get; set; }

        public string? Folio { get; set; }
        //Es el analisis
        public List<AnalysisBkup> AnalysesBkup { get; set; } = new List<AnalysisBkup>();
        public List<Section> Sections { get; set; } = new List<Section>();
        public string ProcessSheet { get; set; }
        public ICollection<Commentary>? ProcessSheetCommentary { get; set; } = new List<Commentary>();
        public ICollection<FileUpload>? CommonDirection { get; set; } = new List<FileUpload>();
        public int? AppliedModelId { get; set; }
        public Product? AppliedModel { get; set; }

        //steps stpes (Puntos Criticos?)


        public ICollection<FileUpload>? Images { get; set; } = new List<FileUpload>();
        public ICollection<FileUpload>? Videos { get; set; } = new List<FileUpload>();
        public string RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<Equipment>? SafetyEquipment { get; set; } = new List<Equipment>();
        public ICollection<Tool>? ToolsUsed { get; set; } = new List<Tool>();
        public ICollection<Material>? MaterialsUsed { get; set; } = new List<Material>();
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public int? StationId { get; set; }
        public Station? Station { get; set; }
        public int? OwnerId { get; set; }
        public User? Owner { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? EditorId { get; set; }
        public User? Editor { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Plan { get; set; }
        public string SourcePlan { get; set; }
        public string Status { get; set; }


        public bool? IsActive { get; set; }
    }
}
