
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
        public string? ProcessSheet { get; set; }
        public ICollection<Commentary>? ProcessSheetCommentary { get; set; } = new List<Commentary>();
        public ICollection<CommonDirection>? CommonDirection { get; set; } = new List<CommonDirection>();

        public List<Product>? AppliedModels { get; set; } = new List<Product>();


        public ICollection<FileUpload>? Images { get; set; } = new List<FileUpload>();
        public ICollection<FileUpload>? Videos { get; set; } = new List<FileUpload>();
        
        public string? RevisedItems { get; set; }
        public int? TrainingTime { get; set; }
        public string? OtherInformation { get; set; }

        public ICollection<Equipment>? SafetyEquipment { get; set; } = new List<Equipment>();
        public List<ToolUsed>? ToolsUsed { get; set; } = new List<ToolUsed>();
        public List<MaterialUsed>? MaterialsUsed { get; set; } = new List<MaterialUsed>();

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
        
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<User>? ApproverOwners { get; set; }
        public List<User>? ReviewerEditors { get; set; }


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

        public List<SOSAnalysis>? SOSAnalysis { get; set; } = new List<SOSAnalysis>();
        public List<SOSCombination>? SOSCombination { get; set; } = new List<SOSCombination>();
        public List<SOSDistribution>? SOSDistribution { get; set; } = new List<SOSDistribution>();
        public List<SOSFlow>? SOSFlow { get; set; } = new List<SOSFlow>();
        public List<SOSSequence>? SOSSequence { get; set; } = new List<SOSSequence>();

        public List<SOSSynopticTableofControlPoints>? SOSSynopticControlPoints { get; set; } = new List<SOSSynopticTableofControlPoints>();
        public List<SOSSynopticTableofOperatingRequirements>? SOSSynopticOperatingRequirements { get; set; } = new List<SOSSynopticTableofOperatingRequirements>();
        public ICollection<SOSSynopticTableRequirementOperationDifficulty>? SOSSynopticOperatingRequirementsDifficulties { get; set; } = new List<SOSSynopticTableRequirementOperationDifficulty>();

        public List<PAT>? PATs { get; set; } = new List<PAT>();

        public int? HciId { get; set; }
        public HCI? Hci { get; set; }

        public bool? IsActive { get; set; }
    }

    public class DifferenceDetail
    {
        public string Property { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}

public class SOSHubDtoList
{
    public int SOSHubId { get; set; }
    public string? Folio { get; set; }
    public string? ProcessSheet { get; set; }
    public bool Selected { get; set; } = false;
}