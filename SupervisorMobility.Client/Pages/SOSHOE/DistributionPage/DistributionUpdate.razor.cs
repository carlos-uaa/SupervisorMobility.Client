using MudBlazor;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
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

namespace SupervisorMobility.Client.Pages.SOSHOE.DistributionPage
{
    public partial class DistributionUpdate
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

        private string[] additionalTimes = new string[] { "0", "0", "0", "0", "0" };
        private string[] cycleTimes = new string[] { "0", "0", "0", "0", "0" };
        private string[] applicationModels = new string[] { "", "", "", "", "" };

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
        //    IndexDistribution = 0;
        //}

        //protected override void OnParametersSet()
        //{
        //    IndexDistribution = 0;
        //}

        public async Task<AsyncVoidMethodBuilder> SetUserInfo()
        {

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


            if (_sosDistribution.Illustrations?.Any() ?? false)
            {

                foreach (var distributionImage in _sosDistribution.Illustrations)
                {
                    var image = await SOSDistributionServices.ShowIlustrationSOSDistribution(distributionImage.FileUploadId);
                    PreviousImages.Add((distributionImage.FileUploadId, image));
                }
            }

            if (_sosDistribution.Notes?.Any() ?? false)
            {
                foreach (var comment in _sosDistribution.Notes)
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

            if (!(_sosDistribution.Notes?.Any() ?? false))
            {
                AddItem();
            }

            cycleId = _sosDistribution.SOSHub.TrainingTime != null ? GetCycleId(_sosDistribution.SOSHub.TrainingTime) : 0;

            if (_sosDistribution.Times == null)
            {
                _sosDistribution.Times = new List<SOSTime>();
                //crearlos artificialmente
                foreach (Section section in _sosDistribution.SOSHub.Sections)
                {
                    SOSTime newitem = new SOSTime();

                    newitem.SectionId = section.SectionId;
                    newitem.IsActive = true;
                    newitem.Time = "";

                    _sosDistribution.Times.Add(newitem);
                }
            }
            else
            {
                //iterar sobre ellos para añadir casos faltantes de haber
                foreach (Section section in _sosDistribution.SOSHub.Sections)
                {
                    if (!_sosDistribution.Times.Any(t => t.SectionId == section.SectionId))
                    {
                        SOSTime newitem = new SOSTime();

                        newitem.SectionId = section.SectionId;
                        newitem.IsActive = true;
                        newitem.Time = "0";

                        _sosDistribution.Times.Add(newitem);
                    }
                }
            }

            var tempAdditionalTimes = _sosDistribution.AdditionalTime?.Split("§") ?? new string[0];
            var tempCycleTimes = _sosDistribution.CycleTime?.Split("§") ?? new string[0];
            var tempApplicationModels = _sosDistribution.AplicationModels?.Split("§") ?? new string[0];

            for (int i = 0; i < 5; i++)
            {
                additionalTimes[i] = i < tempAdditionalTimes.Length && !string.IsNullOrWhiteSpace(tempAdditionalTimes[i]) ? tempAdditionalTimes[i] : "0";
                cycleTimes[i] = i < tempCycleTimes.Length && !string.IsNullOrWhiteSpace(tempCycleTimes[i]) ? tempCycleTimes[i] : "0";
                applicationModels[i] = i < tempApplicationModels.Length && !string.IsNullOrWhiteSpace(tempApplicationModels[i]) ? tempApplicationModels[i] : "";
            }

            totalTime = _sosDistribution.Times
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

        private List<string> GetRevisionNumbers()
        {
            List<string> revisionNumbers = new List<string> { "", "", "" };

            Console.WriteLine(totalLogbooks);

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

                int indexTime = _sosDistribution.Times.ToList().FindIndex(t => t.SectionId == section.SectionId);

                if (indexTime != -1)
                {
                    _sosDistribution.Times.ElementAt(indexTime).Time = newValue;
                }
                else
                {
                    SOSTime newitem = new SOSTime();

                    newitem.SectionId = section.SectionId;
                    newitem.IsActive = true;
                    newitem.Time = newValue;

                    _sosDistribution.Times.Add(newitem);
                }
            }

            totalTime = _sosDistribution.Times
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


        #region Distribution Images

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


        private async Task UpdateDistribution()
        {
            Snackbar.Clear();
            UpdateButton = true;
            await GenerateSOSHUBCommentaries();

            foreach (SOSTime sect in _sosDistribution.Times)
            {
                if (!string.IsNullOrEmpty(sect.Time) && !double.TryParse(sect.Time, out _))
                {
                    Snackbar.Add("The time field only accepts numbers.", Severity.Warning);
                    UpdateButton = false;
                    return;
                }
            }
            var resultSOS = await SOSHubServices.UpdateSOSHub(_sosDistribution.SOSHub);

            if (resultSOS != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"SOS Updated!", Severity.Info);
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");

            var result = await SOSDistributionServices.UpdateSOSDistribution(_sosDistribution);

            if (result != null)
            {
                Snackbar.Add($"Distribution Updated!", Severity.Info);

                _sosDistribution = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/SOSHOE/Distribution");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            UpdateButton = false;


        }

        public async Task<AsyncVoidMethodBuilder> GenerateSOSHUBCommentaries()
        {
            _sosDistribution.Notes?.Clear();
            foreach (var item in items)
            {
                var processSheetCommentary = new Commentary
                {
                    CommentaryId = item.CommentaryId,
                    Comment = item.Commentary,
                    IsActive = item.IsActive
                };

                _sosDistribution.Notes?.Add(processSheetCommentary);
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

                if (!(string.IsNullOrEmpty(processSheetCommentary.Comment)))
                {
                    _sosDistribution.Notes?.Add(processSheetCommentary);
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
            int distributionId = _sosDistribution.SOSDistributionId;

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


                    var result = await SOSDistributionServices.AddIllustrationToSOSDistribution(content, distributionId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Image Added to Distribution", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Image to Distribution", Severity.Error);
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
                    var result = await SOSDistributionServices.RemoveIlustrationFromSOSData(_sosDistribution.SOSDistributionId, file);
                    if (!result)
                    {
                        Snackbar.Add($"Error removing the image", Severity.Error);
                    }
                }
            }

        }

    }
}