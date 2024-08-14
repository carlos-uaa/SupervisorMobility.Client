using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.SOSHOE.SequencePage
{
    public partial class SequenceDetails
    {
        [Parameter]
        public int? SequenceId { get; set; }

        SOSSequence _sosSequence { get; set; } = new();
        private List<SOSSequenceLogbook> mostRecentLogs = new List<SOSSequenceLogbook>();
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


            #region Dummy Data
            var sosHub = new SOSHub
            {
                SOSHubId = 1039,
                ProcessSheet = "testeo",
                AppliedModelId = 1,
                RevisedItems = "testeo",
                TrainingTime = "4 cycles",
                OtherInformation = "testeo",
                PlantId = 1,
                AreaId = 16,
                DepartmentId = 8,
                OwnerId = 83,
                CreatedDate = new DateTime(2024, 8, 9, 16, 41, 25, 273),
                EditorId = 83,
                ModifiedDate = new DateTime(2024, 8, 9, 16, 41, 25, 273),
                Plan = "[Current]",
                SourcePlan = "[Current]",
                Status = "Pending",
                IsActive = true,
                DistributionId = 1384,
                Folio = "C8-Test-L-P71A",
            };

            var sosSequence = new SOSSequence
            {
                SOSSequenceId = 1,
                InternalControlNumber = "ICN123",
                OperationName = "Operation X",
                ProcessName = "Process Y",
                CreatedDate = DateTime.Now,
                IsActive = true,
                SOSHubId = sosHub.SOSHubId,
                SOSHub = sosHub
            };

            var sosSequenceLogbook = new SOSSequenceLogbook
            {
                SOSSequenceLogbookId = 1,
                Status = 1,
                NoRevision = 1,
                IsActive = true,
                SOSAnalysisId = 100, 
                RevisedItem = "Item A",
                SeniorSupervisorId = 200, 
                SupervisorId = 300, 
                Date = DateTime.Now
            };

            sosSequence.SequenceLogbooks.Add(sosSequenceLogbook);
            _sosSequence = sosSequence;
            #endregion
            //_sosSequence = await SOSSequenceServices.GetSOSSequence((int)SequenceId, true, true, true, true, true);
            if (_sosSequence.SequenceLogbooks != null)
            {
                mostRecentLogs = _sosSequence.SequenceLogbooks
                    .OrderByDescending(log => log.SOSSequenceLogbookId)
                    .Take(Math.Min(3, _sosSequence.SequenceLogbooks.Count))
                    .OrderBy(log => log.SOSSequenceLogbookId)
                    .ToList();

                logCount = mostRecentLogs.Count;
                totalLogbooks = _sosSequence.SequenceLogbooks.Count;

            }

            remainingLogs = 3 - logCount;
            logbookIds = mostRecentLogs.Select(log => log.SOSSequenceLogbookId).ToList();



            if (_sosSequence.Illustrations != null && _sosSequence.Illustrations.Any())
            {

                foreach (var sequenceImage in _sosSequence.Illustrations)
                {
                    var image = await SOSSequenceServices.ShowIlustrationSOSSequence(sequenceImage.FileUploadId);
                    capturedImages.Add(image);
                }
            }
            cycleId = _sosSequence.SOSHub?.TrainingTime != null ? GetCycleId(_sosSequence.SOSHub.TrainingTime) : 0;
            if (_sosSequence?.SOSHub?.Sections != null)
            {
                totalTime = _sosSequence.SOSHub.Sections
                    .Select(sect =>
                    {
                        double timeValue;
                        return double.TryParse(sect.Time, out timeValue) ? timeValue : (double?)null;
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
        //    IndexSequence = 0;
        //}

        //protected override void OnParametersSet()
        //{
        //    IndexSequence = 0;
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
            string BaseText = Regex.Replace(_sosSequence.SOSHub?.Sections[sectionIndex].Analyses[analisisIndex].Text, @"\*", "").ToString();

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
            //await Exportation.ExportSequenceToExcel(SequenceId.Value);
        }
    }
}