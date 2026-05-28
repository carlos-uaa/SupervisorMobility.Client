namespace SupervisorMobility.Client.Data.Entities.Dtos
{
    public class SOSState
    {
        public SOSHub Hub { get; set; } = new();

        public List<PAT> Pats { get; set; } = new();
        public List<SOSAnalysis> Analyses { get; set; } = new();
        public List<SOSCombination> Combinations { get; set; } = new();
        public List<SOSDistribution> Distributions { get; set; } = new();
        public List<SOSFlow> Flows { get; set; } = new();
        public List<SOSSequence> Sequences { get; set; } = new();
        public List<SOSSynopticTableofOperatingRequirements> SynopticTableOfOpertingRequirements { get; set; } = new();
        public List<SOSSynopticTableofControlPoints> SynopticTableOfControlPoints { get; set; } = new();
        public HCI Hci { get; set; } = new();

        public List<Plant> Plants { get; set; } = new();
        public User? LogedUser { get; set; }

        public int SelectedIndex { get; set; }
        public bool IsLoading { get; set; }
        public bool Dev_env { get; set; }

        public int? SelectedPlantId { get; set; }
        public int? SelectedAreaId { get; set; }
    }
}
