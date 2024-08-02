using MudBlazor;
using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.JSInterop;
using static MudBlazor.Icons;
using SupervisorMobility.Client.Data.Entities;
using Microsoft.AspNetCore.Components.Web;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.SOSHOE.AnalysisPages
{
    public partial class AnalysisUpdate
    {
        [Parameter]
        public int? AnalysisId { get; set; }

        SOSAnalysis _sosAnalysis { get; set; } = new();
        List<SOSAnalysisLogbook> mostRecentLogs { get; set; }

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

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public int userType = 0;

        //Commentaries
        public class ItemModel
        {
            public int ComentaryId { get; set; }
            public string Commentary { get; set; }
            public bool IsActive { get; set; } = true;

        }

        List<ItemModel> items = new List<ItemModel>();
        List<ItemModel> tempItems = new List<ItemModel>();

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

            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }

            await SetUserInfo();
            ShowLoading = false;
            StateHasChanged();
        }


        //Local storage user
        #region LocalStorageUser
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
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

        public async Task<AsyncVoidMethodBuilder> SetUserInfo()
        {

            _sosAnalysis = await SOSAnalysisServices.GetSOSAnalysis((int)AnalysisId, true, true, true, true, true);
            if (_sosAnalysis.AnalysisLogbooks != null)
                mostRecentLogs = _sosAnalysis.AnalysisLogbooks.Take(Math.Min(3, _sosAnalysis.AnalysisLogbooks.Count)).ToList();
            else
                mostRecentLogs = new List<SOSAnalysisLogbook>();



            if (_sosAnalysis.Notes?.Any() ?? false)
            {
                foreach (var comment in _sosAnalysis.Notes)
                {
                    if (comment.IsActive != null && comment.IsActive == true)
                    {
                        var item = new ItemModel
                        {
                            ComentaryId = comment.ComentaryId,
                            Commentary = comment.Comment,
                        };
                        items.Add(item);
                    }
                }
            }

            AddItem();


            if (_sosAnalysis.Illustrations != null && _sosAnalysis.Illustrations.Any())
            {

                foreach (var analysisImage in _sosAnalysis.Illustrations)
                {
                    var image = await SOSAnalysisServices.ShowIlustrationSOSAnalysis(analysisImage.FileUploadId);
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
            StateHasChanged();

            return new AsyncVoidMethodBuilder();
        }

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

        private string ValidateTime(string time)
        {
            if (string.IsNullOrEmpty(time))
            {
                return null;
            }

            var regex = new Regex(@"^[0-9]*\.?[0-9]*$");
            return regex.IsMatch(time) ? null : "Invalid time format. Only numbers and dots are allowed.";
        }

        private EventCallback<string> CreateValueChangedCallback(Section section)
        {
            return EventCallback.Factory.Create<string>(this, e => UpdateTotal(e, section));
        }

        private void UpdateTotal(string newValue, Section section)
        {
            if (section != null && newValue != null)
            {
                section.Time = newValue;
            }

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
        }

        private async Task UpdateAnalysis()
        {
            Console.WriteLine("uddate");


        }


        //Commentaries
        #region Commentaries
        void AddItem()
        {
            tempItems.Add(new ItemModel());
        }

        void RemoveItem(ItemModel item, bool isTemp)
        {
            Console.WriteLine(item.Commentary);
            if (isTemp)
            {
                Console.WriteLine("a");
            }
            if (isTemp)
            {
                tempItems.Remove(item);
            }
            else
            {
                int index = items.IndexOf(item);

                if (index != -1)
                {
                    items[index].IsActive = false;
                }
            }
        }

        #endregion

    }
}