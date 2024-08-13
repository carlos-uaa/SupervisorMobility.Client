
using Newtonsoft.Json;

namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSHub
    {
        public int SOSHubHistoryId { get; set; }
        public int SOSHubId { get; set; }

        public string? Folio { get; set; }
        //Es el analisis
        public List<AnalysisBkup> AnalysesBkup { get; set; } = new List<AnalysisBkup>();
        public List<Section> Sections { get; set; } = new List<Section>();
        public string ProcessSheet { get; set; }
        public ICollection<Commentary>? ProcessSheetCommentary { get; set; } = new List<Commentary>();
        public ICollection<CommonDirection>? CommonDirection { get; set; } = new List<CommonDirection>();
        public int? AppliedModelId { get; set; }
        public Product? AppliedModel { get; set; }

        //steps stpes (Puntos Criticos?)


        public ICollection<FileUpload>? Images { get; set; } = new List<FileUpload>();
        public ICollection<FileUpload>? Videos { get; set; } = new List<FileUpload>();
        public string RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<Equipment>? SafetyEquipment { get; set; } = new List<Equipment>();
        public List<ToolUsed>? ToolsUsed { get; set; } = new List<ToolUsed>();
        public List<MaterialUsed>? MaterialsUsed { get; set; } = new List<MaterialUsed>();
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

        private string? _versionChanges;

        public string? VersionChanges
        {
            get => _versionChanges;
            set
            {
                _versionChanges = value;
                if (!string.IsNullOrEmpty(_versionChanges))
                {
                    DifferenceDetails = JsonConvert.DeserializeObject<List<DifferenceDetail>>(_versionChanges);
                }
                else
                {
                    DifferenceDetails = new List<DifferenceDetail>();
                }
            }
        }

        public List<DifferenceDetail> DifferenceDetails { get; private set; } = new List<DifferenceDetail>();

        public ICollection<SOSAnalysis>? SOSAnalysis { get; set; } = new List<SOSAnalysis>();
        public ICollection<SOSCombination>? SOSCombination { get; set; } = new List<SOSCombination>();
        public ICollection<SOSDistribution>? SOSDistribution { get; set; } = new List<SOSDistribution>();
        public ICollection<SOSFlow>? SOSFlow { get; set; } = new List<SOSFlow>();
        public ICollection<SOSSequence>? SOSSequence { get; set; } = new List<SOSSequence>();

        public bool? IsActive { get; set; }
    }

    public class DifferenceDetail
    {
        public string Property { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}
