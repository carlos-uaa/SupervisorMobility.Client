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
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace SupervisorMobility.Client.Pages.SOSHOE.AnalysisPages
{
    public partial class AnalysisUpdate
    {
        [Parameter]
        public int? AnalysisId { get; set; }

        SOSAnalysis _sosAnalysis { get; set; } = new();
        List<SOSAnalysisLogbook> mostRecentLogs { get; set; }
        public int logCount = 0;

        int cycleId = 0;
        private List<(int, string)> PreviousImages = new List<(int, string)>();//id, b64 string
        private List<string> capturedImages = new List<string>();
        private List<int> OldImageRemoved = new();

        //Show evidence
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;


        //Commentaries and Logbok
        private DialogOptions dialogCommentariesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogLogbookOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogImagesOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        private bool visibleImagesDialog = false;
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
            public int CommentaryId { get; set; }
            public string Commentary { get; set; }
            public bool IsActive { get; set; } = true;

        }

        List<ItemModel> items = new List<ItemModel>();
        List<ItemModel> tempItems = new List<ItemModel>();

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;
        public bool UpdateButton = false;


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
            {

                mostRecentLogs = _sosAnalysis.AnalysisLogbooks.Take(Math.Min(3, _sosAnalysis.AnalysisLogbooks.Count)).ToList();
                logCount = mostRecentLogs.Count;
            }
            else
                mostRecentLogs = new List<SOSAnalysisLogbook>();


            if (_sosAnalysis.Illustrations?.Any() ?? false)
            {

                foreach (var analysisImage in _sosAnalysis.Illustrations)
                {
                    var image = await SOSAnalysisServices.ShowIlustrationSOSAnalysis(analysisImage.FileUploadId);
                    PreviousImages.Add((analysisImage.FileUploadId, image));
                }
            }

            if (_sosAnalysis.Notes?.Any() ?? false)
            {
                foreach (var comment in _sosAnalysis.Notes)
                {
                    if (comment.IsActive != null && comment.IsActive == true)
                    {
                        var item = new ItemModel
                        {
                            CommentaryId = comment.CommentaryId,
                            Commentary = comment.Comment,
                        };
                        items.Add(item);
                    }
                }
            }

            AddItem();

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


        #region Analysis Images

        private void RemoveOldImage(int fileId)
        {
            OldImageRemoved.Add(fileId);
            PreviousImages.RemoveAll(p => p.Item1 == fileId);
        }

        private void OpenEvidenceDialog(int index, bool OldImgOrigin = true)
        {
            if (!OldImgOrigin) index += (PreviousImages.Count);
            photoIndex = index;
            visibleEvidence = true;

        }

        public void ShowImagesDialog()
        {
            visibleImagesDialog = true;
        }

        void CloseImagesDialog()
        {
            visibleImagesDialog = false;
        }

        private void RemoveImage(int index, int imgIndex)
        {

            if (index >= 0 && index < capturedImages.Count)
            {
                if (imgIndex == 1)
                {
                    capturedImages.RemoveAt(index);
                }
            }
        }
        #endregion


        private async Task UpdateAnalysis()
        {
            Snackbar.Clear();
            UpdateButton = true;
            await GenerateSOSHUBCommentaries();

            foreach (Section sect in _sosAnalysis.SOSHub.Sections)
            {
                if (!string.IsNullOrEmpty(sect.Time) && !double.TryParse(sect.Time, out _))
                {
                    Snackbar.Add("The time field only accepts numbers.", Severity.Warning);
                    UpdateButton = false;
                    return;
                }
            }
            //var resultSOS = await SOSHubServices.UpdateSOSHub(_sosAnalysis.SOSHub);

            //if (resultSOS != null)
            //{
            //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //    Snackbar.Add($"SOS Updated!", Severity.Info);
            //}
            //else
            //    await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");

            var result = await SOSAnalysisServices.UpdateSOSAnalysis(_sosAnalysis);

            if (result != null)
            {
                Snackbar.Add($"Analysis Updated!", Severity.Info);

                _sosAnalysis = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/SOSHOE/Analysis");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            UpdateButton = false;


        }

        public async Task<AsyncVoidMethodBuilder> GenerateSOSHUBCommentaries()
        {
            _sosAnalysis.Notes?.Clear();
            foreach (var item in items)
            {
                var processSheetCommentary = new Commentary
                {
                    CommentaryId = item.CommentaryId,
                    Comment = item.Commentary,
                    IsActive = item.IsActive
                };

                _sosAnalysis.Notes?.Add(processSheetCommentary);
            }

            int i = 0;
            foreach (var item in tempItems)
            {
                i++;
                var processSheetCommentary = new Commentary
                {
                    CommentaryId = 0,
                    Comment = item.Commentary,
                    IsActive = true
                };

                if (!(string.IsNullOrEmpty(processSheetCommentary.Comment) && i == 1))
                {
                    _sosAnalysis.Notes?.Add(processSheetCommentary);
                }

            }

            return new AsyncVoidMethodBuilder();
        }

        private async Task<AsyncVoidMethodBuilder> UploadEvidence()
        {
            await UploadImages();
            await UpdateRemovedFiles();

            return new AsyncVoidMethodBuilder();
        }

        private async Task UploadImages()
        {

            List<string> images = capturedImages;
            int analysisId = _sosAnalysis.SOSAnalysisId;

            if (images.Count > 0)
            {
                foreach (var imageData in images)
                {
                    if (string.IsNullOrEmpty(imageData))
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("No image data to upload", Severity.Warning);
                        continue;
                    }

                    string base64Data = "";
                    if (imageData.Contains("data:image/png;base64,"))
                    {
                        base64Data = imageData.Replace("data:image/png;base64,", "");
                    }
                    else if (imageData.Contains("data:image/jpeg;base64,"))
                    {
                        base64Data = imageData.Replace("data:image/jpeg;base64,", "");
                    }
                    else if (imageData.Contains("data:image/jpg;base64,"))
                    {
                        base64Data = imageData.Replace("data:image/jpg;base64,", "");
                    }
                    else if (imageData.Contains("data:image/gif;base64,"))
                    {
                        base64Data = imageData.Replace("data:image/gif;base64,", "");
                    }
                    else if (imageData.Contains("data:image/svg+xml;base64,"))
                    {
                        base64Data = imageData.Replace("data:image/svg+xml;base64,", "");
                    }

                    if (!IsValidBase64String(base64Data))
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Invalid image data", Severity.Error);
                        continue;
                    }

                    var imageBytes = Convert.FromBase64String(base64Data);

                    using var content = new MultipartFormDataContent();
                    var imageStream = new MemoryStream(imageBytes);
                    var fileContent = new StreamContent(imageStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                    content.Add(fileContent, "\"file\"", "evidenceSosHub.png");


                    var result = await SOSAnalysisServices.AddIllustrationToSOSAnalysis(content, analysisId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Image Added to Analysis", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Image to Analysis", Severity.Error);
                    }

                }

                images.Clear();
            }

        }

        private bool IsValidBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private async Task AddImages(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles())
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    using (Stream mediaStream = file.OpenReadStream(file.Size))
                    {
                        MemoryStream ms = new();
                        await mediaStream.CopyToAsync(ms);
                        string mediaUri = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

                        capturedImages.Add(mediaUri);
                    }
                }
            }
        }

        private async Task UpdateRemovedFiles()
        {
            Snackbar.Clear();
            if (OldImageRemoved.Any())
            {
                foreach (var file in OldImageRemoved)
                {
                    var result = await SOSAnalysisServices.RemoveIlustrationFromSOSData(_sosAnalysis.SOSAnalysisId, file);
                    if (result)
                    {
                        Snackbar.Add($"Error removing the image", Severity.Error);
                    }
                }
            }

        }
    }
}