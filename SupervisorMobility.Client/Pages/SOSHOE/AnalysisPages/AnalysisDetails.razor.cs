using MudBlazor;
using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.SOSHOE.AnalysisPages
{
    public partial class AnalysisDetails
    {
        [Parameter]
        public int? AnalysisId { get; set; }

        SOSAnalysis _sosAnalysis { get; set; } = new();
        int cycleId = 0;
        private List<string> capturedImages = new List<string>();


        //Show evidence
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;


        //Commentaries and Logbok
        private DialogOptions dialogCommentariesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogLogbookOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleCommentaries = false;
        private bool visibleLogbook = false;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;


        private double totalTime;
        // Dummy data
        public List<SOSAnalysisLogbook> exampleLogbooks = new List<SOSAnalysisLogbook>
                    {
                        new SOSAnalysisLogbook { RevisedItem = "Jose Abdala 29 de Enero", Date = new DateTime(2024, 1, 29) },
                        new SOSAnalysisLogbook { RevisedItem = "Jose Abdala 15 de Marzo", Date = new DateTime(2024, 3, 15) },
                        new SOSAnalysisLogbook { RevisedItem = "Jose Abdala 10 de Agosto", Date = new DateTime(2024, 8, 10) }
                    };

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
            totalTime = _sosAnalysis.SOSHub.Sections
                .Select(sect =>
                {
                    double timeValue;
                    return double.TryParse(sect.Time, out timeValue) ? timeValue : (double?)null;
                })
                .Where(timeValue => timeValue.HasValue)
                .Select(timeValue => timeValue.Value)
                .DefaultIfEmpty(0)
                .Sum();
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
        //protected override void OnAfterRender(bool firstRender)
        //{
        //    IndexAnalysis = 0;
        //}

        //protected override void OnParametersSet()
        //{
        //    IndexAnalysis = 0;
        //}


        public static string ReasonFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (!input.StartsWith("("))
            {
                input = "(" + input;
            }

            if (!input.EndsWith(")"))
            {
                input = input + ")";
            }

            return input;
        }

        private void OpenEvidenceDialog(int index)
        {
            photoIndex = index;
            visibleEvidence = true;

        }

        private void OpenCommentariesDialog()
        {
            visibleCommentaries = true;

        }

        private void CloseCommentariesDialog()
        {
            visibleCommentaries = false;
        }

        private void OpenLogbookDialog()
        {
            visibleLogbook = true;

        }

        private void CloseLogbookDialog()
        {
            visibleLogbook = false;
        }


        //Highlight critical points

        private string GetFormatedAnalisisText(int sectionIndex, int analisisIndex)
        {
            string BaseText = Regex.Replace(_sosAnalysis.SOSHub?.Sections[sectionIndex].Analyses[analisisIndex].Text, @"\*", "").ToString();

            return BaseText;
        }


        private MarkupString GenerateHighlightedText(string text, List<string> criticalPoints)
        {
            if (string.IsNullOrEmpty(text) || criticalPoints == null || criticalPoints.Count == 0)
            {
                return new MarkupString(text);
            }

            var normalizedText = Normalize(text);
            var builder = new StringBuilder();
            var currentIndex = 0;

            foreach (var criticalPoint in criticalPoints)
            {
                var normalizedCriticalPoint = Normalize(criticalPoint);
                var match = Regex.Match(normalizedText, Regex.Escape(normalizedCriticalPoint), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    var startIndex = match.Index;
                    var endIndex = startIndex + criticalPoint.Length;

                    builder.Append(text.Substring(currentIndex, startIndex - currentIndex));

                    builder.Append($"<mark>{text.Substring(startIndex, endIndex - startIndex)}</mark>");

                    currentIndex = endIndex;
                }
            }

            builder.Append(text.Substring(currentIndex));

            return new MarkupString(builder.ToString());
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString().ToLowerInvariant();
        }


    }
}