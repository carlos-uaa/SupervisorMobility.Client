using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Pages.SOSHOE.AnalysisPages
{
    public partial class AnalysisDetails
    {
        [Parameter]
        public int? AnalysisId { get; set; }

        SOSAnalysis _sosAnalysis { get; set; } = new();
        private List<SOSAnalysisLogbook> mostRecentLogs = new List<SOSAnalysisLogbook>();
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

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

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

            logged = await HasPropertyAsync();

            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {

                _sosAnalysis = await SOSAnalysisServices.GetSOSAnalysis((int)AnalysisId, true, true, true, true, true);
                if (_sosAnalysis.AnalysisLogbooks != null)
                {
                    mostRecentLogs = _sosAnalysis.AnalysisLogbooks
                        .OrderByDescending(log => log.SOSAnalysisLogbookId)
                        .Take(Math.Min(3, _sosAnalysis.AnalysisLogbooks.Count))
                        .OrderBy(log => log.SOSAnalysisLogbookId)
                        .ToList();

                    logCount = mostRecentLogs.Count;
                    totalLogbooks = _sosAnalysis.AnalysisLogbooks.Count;

                }

                remainingLogs = 3 - logCount;
                logbookIds = mostRecentLogs.Select(log => log.SOSAnalysisLogbookId).ToList();



                if (_sosAnalysis.Illustrations != null && _sosAnalysis.Illustrations.Any())
                {

                    foreach (var analysisImage in _sosAnalysis.Illustrations)
                    {
                        var image = await SOSAnalysisServices.ShowIlustrationSOSAnalysis(analysisImage.FileUploadId);
                        capturedImages.Add(image);
                    }
                }
                cycleId = _sosAnalysis.SOSHub?.TrainingTime ?? 0;

                //creacion artificial
                if (_sosAnalysis.Times == null)
                {
                    _sosAnalysis.Times = new List<SOSTime>();
                    foreach (Section section in _sosAnalysis.SOSHub.Sections)
                    {
                        SOSTime newitem = new SOSTime();

                        newitem.SectionId = section.SectionId;
                        newitem.IsActive = true;
                        newitem.Time = "0";

                        _sosAnalysis.Times.Add(newitem);
                    }
                }
                else
                {
                    //iterar sobre existentes para a�adir casos faltantes de haber
                    foreach (Section section in _sosAnalysis.SOSHub.Sections)
                    {
                        if (!_sosAnalysis.Times.Any(t => t.SectionId == section.SectionId))
                        {
                            SOSTime newitem = new SOSTime();

                            newitem.SectionId = section.SectionId;
                            newitem.IsActive = true;
                            newitem.Time = "0";

                            _sosAnalysis.Times.Add(newitem);
                        }
                    }
                }


                totalTime = _sosAnalysis.Times
                    .Select(time =>
                    {
                        double timeValue;
                        return double.TryParse(time.Time, out timeValue) ? timeValue : (double?)null;
                    })
                    .Where(timeValue => timeValue.HasValue)
                    .Select(timeValue => timeValue.Value)
                    .DefaultIfEmpty(0)
                    .Sum();

                

                ShowLoading = false;
                StateHasChanged();
            }

        }
        #region User
        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        #endregion

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

            var builder = new StringBuilder();
            var currentIndex = 0;
            var matches = new List<Match>();

            foreach (var criticalPoint in criticalPoints)
            {
                var escaped = Regex.Escape(criticalPoint);
                matches.AddRange(Regex.Matches(text, escaped, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Cast<Match>());
            }

            // Ordenar por posición en el texto
            var orderedMatches = matches
                .OrderBy(m => m.Index)
                .Aggregate(new List<Match>(), (acc, match) =>
                {
                    if (acc.Count == 0 || match.Index >= acc.Last().Index + acc.Last().Length)
                    {
                        acc.Add(match);
                    }
                    return acc;
                });

            foreach (var match in orderedMatches)
            {
                if (match.Index > currentIndex)
                {
                    builder.Append(text.Substring(currentIndex, match.Index - currentIndex));
                }

                builder.Append($"<mark>{text.Substring(match.Index, match.Length)}</mark>");
                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < text.Length)
            {
                builder.Append(text.Substring(currentIndex));
            }

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
            await Exportation.ExportAnalysisToExcel(AnalysisId.Value);
        }

        #region ApproveAnalysis

        private bool visibleSign = false;
        private void OpenSignComment()
        {
            visibleSign = true;
        }
        void CloseSign() => visibleSign = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        public async Task ApprovePat()
        {

            //_pat!.Status = 2;
            //var result = await PATsServices.UpdatePat(_pat);

            //if (result)
            //{
            //    Snackbar.Clear();
            //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //    Snackbar.Add($"PAT Approved succesfully!", Severity.Info);
            //    NavigationManager.NavigateTo($"PAT");
            //}

            visibleSign = false;
        }
        #endregion

        void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }
        void UpdateAnalysis(int AnalysisId)
        {
            NavigationManager.NavigateTo($"/soshoe/Analysis/Update/{AnalysisId}");
        }
    }
}