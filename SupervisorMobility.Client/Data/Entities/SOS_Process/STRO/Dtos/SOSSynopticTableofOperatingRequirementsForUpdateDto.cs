namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Dtos
{
    public class SOSSynopticTableofOperatingRequirementsForUpdateDto
    {
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }

        public int? CreatorId { get; set; }
        public int? ReviewerId { get; set; }
        public int? ApproverId { get; set; }

        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }

        public List<int>? SOSHubIds { get; set; } = new List<int>();
        public ICollection<SOSSynopticRequirementsOperationSequence>? SOSSynopticRequirementsOperationSequence { get; set; }
        public List<SOSSynopticTableRequirementOperationDifficulty>? RequirementDifficulties { get; set; } = new List<SOSSynopticTableRequirementOperationDifficulty>();

        public ICollection<SOSSTROKnowledgeHub>? SOSSTROKnowledge { get; set; } = new List<SOSSTROKnowledgeHub>();
        public ICollection<SOSSTROSkillHub>? SOSSTROSkill { get; set; } = new List<SOSSTROSkillHub>();
        public ICollection<EstablishedConditions>? EstablishedConditions { get; set; } = new List<EstablishedConditions>();
        public ICollection<InsuranceFeatures>? InsuranceFeatures { get; set; } = new List<InsuranceFeatures>();
        public ICollection<OperationMachine>? OperationMachine { get; set; } = new List<OperationMachine>();
    }
}