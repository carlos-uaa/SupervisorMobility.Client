using BlazorCameraStreamer;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using SupervisorMobility.Client.Pages.Configuration.ProductPage;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages.Inicio.HOEPage
{
    public partial class CreateHOE
    {

        private SOSHub _sosHub = new();

        private List<BreadcrumbItem> _links;
        private DateTime? createdDateTime = DateTime.Now;
        private DateTime? modifiedDateTime = DateTime.Now;

        public string hour1 { get; set; }
        TimeSpan? startHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;
        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public int userType = 0;
        public string otherInformation = "In case of doubt contact supervisor or leader and stop, call and wait." +
            "Use bare hands or lint free gloves when attaching the rubber gasket. Do not re-use the water pump gasket." +
            " Do not use dropped gaskets.";

        private List<Segment> segments = new List<Segment>();
        private List<Product> _products = new List<Product>();

        public int productId = 0;
        public class Segment
        {
            public string MainPoint { get; set; }
            public List<string> CriticalPoints { get; set; } = new List<string>();
        }

        private bool visibleStepsDialog = false;
        private bool visibleImagesDialog = false;
        private bool visibleVideosDialog = false;


        private DialogOptions dialogStepsOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true, CloseButton = true };
        
        private DialogOptions dialogImagesOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true, CloseButton = true };
        private DialogOptions dialogVideosOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


        //Videos
        public Dictionary<string, (string, string)> MediaUris = new Dictionary<string, (string, string)>();
        private bool DisabledFinish;

        //Commentaries
        public class ItemModel
        {
            public string Commentary { get; set; }
        }

        List<ItemModel> items = new List<ItemModel>();


        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "", disabled: true)
                };
            BreadcrumbServices.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }

            _products = await ProductsServices.GetProducts();
            AddItem();

            StateHasChanged();
        }



        //Local storage user
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



        public void AnalyzeText()
        {
            segments.Clear();

            var segmentTexts = _sosHub.OperationDescription.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segmentText in segmentTexts)
            {
                var segment = new Segment();

                var regex = new Regex(@"\*(.*?)\*");
                var matches = regex.Matches(segmentText);

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        segment.CriticalPoints.Add(match.Groups[1].Value);
                    }
                }

                segment.MainPoint = regex.Replace(segmentText, string.Empty).Trim();
                segments.Add(segment);
            }
        }


        public void ShowStepsDialog()
        {
            visibleStepsDialog = true;
        }

        void CloseStepsDialog()
        {
            visibleStepsDialog = false;
        }

        public void ShowImagesDialog()
        {
            visibleImagesDialog = true;
        }

        void CloseImagesDialog()
        {
            visibleImagesDialog = false;
        }

        //Camera
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new List<string>();

        private bool visibleCamera = false;
        private int imageIndex = 0;

        private void OpenCameraDialog(int index)
        {
            imageIndex = index;
            visibleCamera = true;
        }

        private CameraStreamer CameraStreamerReference;

        private string? cameraId = null;

        private int frameCount;

        private string imageData;

        private async void OnRenderedHandler()
        {
            frameCount = 0;

            if (await CameraStreamerReference.GetCameraAccessAsync())
            {
                await CameraStreamerReference.ReloadAsync();

            }
        }

        private async void Stop()
        {
            await CameraStreamerReference.StopAsync();
        }

        private void OnFrameHandler(string _)
        {
            ++frameCount;
        }

        private async void GetCurrentFrame()
        {
            imageData = await CameraStreamerReference.GetCurrentFrameAsync();

            if (!string.IsNullOrEmpty(imageData))
            {
                if (imageIndex == 1)
                {
                    capturedImages.Add(imageData);
                }
            }
            visibleCamera = false;
            StateHasChanged();
            Stop();
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

        private async Task UploadImages(List<string> images, int kaizenId, bool isPrevious)
        {

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

                    content.Add(fileContent, "\"file\"", "evidence.png");


                    //var result = isPrevious
                    // ? await FilesServices.UploadEvidencesKaizenPrevious(content, kaizenId)
                    // : await FilesServices.UploadEvidencesKaizenThen(content, kaizenId);

                    //if (result is not null)
                    //{
                    //    Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                    //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    //    Snackbar.Add("Image Added to Kaizen", Severity.Info);
                    //}
                    //else
                    //{
                    //    Snackbar.Clear();
                    //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    //    Snackbar.Add("Failed to upload Image to Kaizen", Severity.Error);
                    //}

                }

                images.Clear();
            }

        }

        private async Task<AsyncVoidMethodBuilder> UploadEvidence()
        {
            //await UploadImages(capturedImages, _kaizen.KaizenId, true);
            //await UploadImages(capturedImagesThen, _kaizen.KaizenId, false);

            return new AsyncVoidMethodBuilder();
        }

        private async Task UploadFiles(InputFileChangeEventArgs e, int type)
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

                        if (type == 1)
                        {
                            capturedImages.Add(mediaUri);
                        }
                    }
                }
            }
        }
        //Show Evidence 
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;
        private void OpenEvidenceDialog(int index, int evidenceIndex)
        {
            photoIndex = index;
            visibleEvidence = true;

        }

        //Videos
        public void ShowVideosDialog()
        {
            visibleVideosDialog = true;
        }

        void CloseVideosDialog()
        {
            visibleVideosDialog = false;
        }

        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            List<string[]> IgnoredFiles = new List<string[]>();
            string message;

            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                string contentType = "";

                if (!file.ContentType.StartsWith("video/"))
                {
                    IgnoredFiles.Add(new string[] { file.Name, "Wrong Type" });
                    continue;
                }
                //content.Clear();
                //content = await GetDataTableFromExcel(file);
                if (!MediaUris.Keys.Contains(file.Name))
                {
                    using (Stream mediaStream = file.OpenReadStream(file.Size))
                    {
                        MemoryStream ms = new();
                        await mediaStream.CopyToAsync(ms);
                        string MediaUri = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

                        MediaUris.Add(file.Name, (file.ContentType, MediaUri));
                    }
                }
            }
            //SetDragClass();
            //ClearDragClass();
            if (IgnoredFiles.Count != 0)
            {
                string confirmMessage = $"The following files where ignored: ";
                foreach (string[] item in IgnoredFiles)
                {
                    confirmMessage += $"\n Name: {item[0]} \n Reason: {item[1]}";
                }
                await JSRuntime.InvokeAsync<bool>("confirm", confirmMessage);
            }

            StateHasChanged();
        }

        public async Task Submit()
        {
            DisabledFinish = true;

            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            using var content = new MultipartFormDataContent();

            foreach (var item in MediaUris)
            {
                string base64Data = item.Value.Item2.Split(',')[1];
                if (string.IsNullOrEmpty(base64Data))
                {
                    FileErrorMessage(item.Key);
                    continue;
                }

                try
                {
                    var imageBytes = Convert.FromBase64String(base64Data);
                    var imageStream = new MemoryStream(imageBytes);
                    imageStream.Position = 0;
                    var fileContent = new StreamContent(imageStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.Value.Item1);

                    content.Add(
                        content: fileContent,
                        name: "Files",
                        fileName: item.Key);
                }
                catch (FormatException)
                {
                    FileErrorMessage(item.Key);
                    continue;
                }
                catch (Exception)
                {
                    Snackbar.Add($"Error converting video to uploadable data: {item.Key}", Severity.Error);
                }

            }

            var response = await TestService.UploadVideoFiles(content);

            switch (response.Item1)
            {
                case 500:
                    Snackbar.Add("Error uploading files ", Severity.Error);
                    Console.WriteLine(response.Item2);
                    break;
                case 400:
                    Snackbar.Add("Error uploading some files ", Severity.Warning);
                    Console.WriteLine(response.Item2);
                    break;
                case 200:
                    Snackbar.Add("Success uploading files ", Severity.Success);
                    Console.WriteLine(response.Item2);
                    break;
            }
            DisabledFinish = false;
            //PirService.CreatePIR(pir);

            //NotiService.CreateNotification(notification);
        }

        private void FileErrorMessage(string file)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"File: {file} has invalid video data", Severity.Error);
        }

        private void DeleteItem(string key)
        {
            Console.WriteLine("Before deletion:");
            foreach (var uri in MediaUris)
            {
                Console.WriteLine($"{uri.Key}");
            }

            if (MediaUris.ContainsKey(key))
            {
                MediaUris.Remove(key);
                Console.WriteLine($"Deleted: {key}");
                StateHasChanged();
            }

            Console.WriteLine("After deletion:");
            foreach (var uri in MediaUris)
            {
                Console.WriteLine($"{uri.Key}");
            }

            InvokeAsync(StateHasChanged);
        }


        //Commentaries
        void AddItem()
        {
            items.Add(new ItemModel());
        }

        void RemoveItem(ItemModel item)
        {
            if (items.Count > 1)
            {
                items.Remove(item);
            }

        }

        private async Task CreateNewSOSHub()
        {

            _sosHub.AppliedModelId = productId;

            if (!(items == null || !items.Any()))
            {
                foreach (var item in items)
                {
                    var processSheetCommentary = new Commentary
                    {
                        ComentaryId = 0,
                        Comment = item.Commentary,
                        IsActive = true
                    };
                    _sosHub.ProcessSheetCommentary.Add(processSheetCommentary);
                }
            }

            //var result = await SOSServices.CreateSOSHub(_sosHub);

            //if (result != null)
            //{
            //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //    Snackbar.Add($"SOS Created", Severity.Info);

            //    _sosHub = result;
            //    _ = await UploadEvidence();

            //    NavigationManager.NavigateTo("/sosHub");
            //}
            //else
            //    await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

        }


    }
}