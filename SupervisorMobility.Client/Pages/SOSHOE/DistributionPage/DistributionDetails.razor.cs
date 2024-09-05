using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using SupervisorMobility.Client.Data.Entities.SOS_Process;

namespace SupervisorMobility.Client.Pages.SOSHOE.DistributionPage
{
    public partial class DistributionDetails
    {
        [Parameter]
        public int? DistributionId { get; set; }

        SOSDistribution _sosDistribution { get; set; } = new();
        private List<SOSDistributionLogbook> mostRecentLogs = new List<SOSDistributionLogbook>();
        private int logCount = 0;
        private int totalLogbooks = 0;
        private int remainingLogs = 0;
        private List<int> logbookIds = new List<int>();

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

            _sosDistribution = await SOSDistributionServices.GetSOSDistribution((int)DistributionId, true, true, true, true, true);
            if (_sosDistribution.DistributionLogbooks != null)
            {
                mostRecentLogs = _sosDistribution.DistributionLogbooks
                    .OrderByDescending(log => log.SOSDistributionLogbookId)
                    .Take(Math.Min(3, _sosDistribution.DistributionLogbooks.Count))
                    .OrderBy(log => log.SOSDistributionLogbookId)
                    .ToList();

                logCount = mostRecentLogs.Count;
                totalLogbooks = _sosDistribution.DistributionLogbooks.Count;

            }

            remainingLogs = 3 - logCount;
            logbookIds = mostRecentLogs.Select(log => log.SOSDistributionLogbookId).ToList();



            if (_sosDistribution.Illustrations != null && _sosDistribution.Illustrations.Any())
            {

                foreach (var distributionImage in _sosDistribution.Illustrations)
                {
                    var image = await SOSDistributionServices.ShowIlustrationSOSDistribution(distributionImage.FileUploadId);
                    capturedImages.Add(image);
                }
            }
            cycleId = _sosDistribution.SOSHub?.TrainingTime != null ? GetCycleId(_sosDistribution.SOSHub.TrainingTime) : 0;

            //creacion artificial
            if (_sosDistribution.Times == null)
            {
                _sosDistribution.Times = new List<SOSTime>();
                foreach (Section section in _sosDistribution.SOSHub.Sections)
                {
                    SOSTime newitem = new SOSTime();

                    newitem.SectionId = section.SectionId;
                    newitem.IsActive = true;
                    newitem.Time = "0";

                    _sosDistribution.Times.Add(newitem);
                }
            }
            else
            {
                //iterar sobre existentes para añadir casos faltantes de haber
                foreach (Section section in _sosDistribution.SOSHub.Sections)
                {
                    if (!_sosDistribution.Times.Any(t => t.SectionId == section.SectionId))
                    {
                        SOSTime newitem = new SOSTime();

                        newitem.SectionId = section.SectionId;
                        newitem.IsActive = true;
                        newitem.Time = "";

                        _sosDistribution.Times.Add(newitem);
                    }
                }
            }





            if (_sosDistribution?.SOSHub?.Sections != null)
            {
                totalTime = _sosDistribution.Times
                    .Select(time =>
                    {
                        double timeValue;
                        return double.TryParse(time.Time, out timeValue) ? timeValue : (double?)null;
                    })
                    .Where(timeValue => timeValue.HasValue)
                    .Select(timeValue => timeValue.Value)
                    .DefaultIfEmpty(0)
                    .Sum();
            }



            ShowLoading = false;
            StateHasChanged();
        }

        private List<string> GetRevisionNumbers()
        {
            List<string> revisionNumbers = new List<string> { "", "", "" };

            if (totalLogbooks <= 3)
            {
                for (int i = 0; i < totalLogbooks; i++)
                {
                    if (i == 0)
                    {
                        revisionNumbers[0] = "N";
                    }
                    else
                    {
                        revisionNumbers[i] = (i).ToString();
                    }
                }
            }
            else
            {
                revisionNumbers[0] = (totalLogbooks - 3).ToString();
                revisionNumbers[1] = (totalLogbooks - 2).ToString();
                revisionNumbers[2] = (totalLogbooks - 1).ToString();
            }

            return revisionNumbers;
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
        //    IndexDistribution = 0;
        //}

        //protected override void OnParametersSet()
        //{
        //    IndexDistribution = 0;
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
            string BaseText = Regex.Replace(_sosDistribution.SOSHub?.Sections[sectionIndex].Analyses[analisisIndex].Text, @"\*", "").ToString();

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

        private async void DownloadExcel()
        {
            //await Exportation.ExportDistributionToExcel(DistributionId.Value);
        }
    }
}