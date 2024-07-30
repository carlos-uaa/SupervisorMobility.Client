using MudBlazor;
namespace SupervisorMobility.Client.Pages.SOSHOE.AnalysisPages
{
    public partial class AnalysisDetailsRedesign
    {
        [Parameter]
        public int? AnalysisId { get; set; }
        SOSAnalysis _sosAnalysis { get; set; } = new();
        List<SOSAnalysisLogbook> mostRecentLogs { get; set; }
        int cycleId = 0;
        private List<string> capturedImages = new List<string>();

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        public bool ShowLoading = true;
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        protected async override Task OnInitializedAsync()
        {

            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");

            _sosAnalysis = await SOSAnalysisServices.GetSOSAnalysis((int)AnalysisId, true, true, true, true, true);

            if(_sosAnalysis.AnalysisLogbooks!=null)
                mostRecentLogs = _sosAnalysis.AnalysisLogbooks.Take(Math.Min(3, _sosAnalysis.AnalysisLogbooks.Count)).ToList();
            else
                mostRecentLogs = new List<SOSAnalysisLogbook>();

            Console.WriteLine("test");

            if (_sosAnalysis.Illustrations != null && _sosAnalysis.Illustrations.Any())
            {
                Console.WriteLine("aaa");

                foreach (var analysisImage in _sosAnalysis.Illustrations)
                {
                    var image = await SOSAnalysisServices.ShowIlustrationSOSAnalysis(analysisImage.FileUploadId);
                    Console.WriteLine("aaa");
                    capturedImages.Add(image);
                }
            }
            cycleId = _sosAnalysis.SOSHub.TrainingTime != null ? GetCycleId(_sosAnalysis.SOSHub.TrainingTime) : 0;

            ShowLoading = false;
            StateHasChanged();
        }

        public static int GetCycleId(string trainingTime)
        {
            string cycleIdString = trainingTime.Split(' ').First();

            if (int.TryParse(cycleIdString, out int cycleId))
            {
                return cycleId;
            }
            else
            {
                return 0;
            }
        }
    }
}