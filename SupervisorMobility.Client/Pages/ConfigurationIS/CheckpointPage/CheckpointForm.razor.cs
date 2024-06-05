using BlazorCameraStreamer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;


namespace SupervisorMobility.Client.Pages.ConfigurationIS.CheckpointPage
{
    public partial class CheckpointForm
    {
        [Parameter]
        public int? CheckpointId { get; set; }

        public Checkpoint _Checkpoint { get; set; } = new Checkpoint();

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;


        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public enum PageType
        {
            Details,
            Create,
            Update,
            Another
        }

        public PageType pageType { get; set; }

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavigationManager.Uri;

            pageType = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase) ? PageType.Details : PageType.Another;

            if (pageType == PageType.Another)
            {
                pageType = currentUrl.Contains("Create", StringComparison.OrdinalIgnoreCase) ? PageType.Create : PageType.Another;
            }

            if (pageType == PageType.Another)
            {
                pageType = currentUrl.Contains("Update", StringComparison.OrdinalIgnoreCase) ? PageType.Update : PageType.Another;
            }


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


            _links = new List<BreadcrumbItem>
             {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                new BreadcrumbItem(text: Localizer["Checkpoints"], href: "/configurationIS/Checkpoints")
            };


            await GetUserAsync();
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
                try
                {

                    switch (pageType)
                    {
                        case PageType.Details:
                            if (CheckpointId != null)
                            {
                                _Checkpoint = await _CheckPointServices.GetCheckpoint((int)CheckpointId, true);
                                _links.Add(new BreadcrumbItem(text: Localizer["Details"], href: $"/configurationIS/Checkpoints/{CheckpointId}", disabled: true));
                                _links.Add(new BreadcrumbItem(text: _Checkpoint.CheckpointTitle, href: $"/configurationIS/Checkpoints/{CheckpointId}", disabled: true));
                            }
                            break;
                        case PageType.Create:
                            _links.Add(new BreadcrumbItem(text: Localizer["Create"], href: $"/configurationIS/Checkpoints/", disabled: true));
                            _Checkpoint.IsActive = true;
                            break;

                        case PageType.Update:
                            if (CheckpointId != null)
                            {
                                _Checkpoint = await _CheckPointServices.GetCheckpoint((int)CheckpointId, true);
                                //_Checkpoint 
                                _links.Add(new BreadcrumbItem(text: Localizer["Update"], href: $"/configurationIS/Checkpoints/", disabled: true));
                                _links.Add(new BreadcrumbItem(text: _Checkpoint.CheckpointTitle, href: $"/configurationIS/Checkpoints/{CheckpointId}", disabled: true));
                            }
                            break;
                    }

                    if (pageType != PageType.Create)
                    {
                        if (_Checkpoint.Sketches != null && _Checkpoint.Sketches.Count > 0)
                        {
                            foreach (var evidence in _Checkpoint.Sketches)
                            {
                                var imageUrl = await _CheckPointServices.ShowImageCheckpoint(evidence.FileUploadId);
                                CheckpointImages.Add(imageUrl);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    BreadcrumbService.UpdateBreadcrumbs(_links);
                    ShowLoading = false;
                    base.StateHasChanged();
                }
            }



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

        private async void SubmitOperations()
        {
            switch (pageType)
            {
                case PageType.Create:

                    var resultCreate = await _CheckPointServices.CreateCheckpoint(_Checkpoint);

                    if (resultCreate != null)
                    {
                        _Checkpoint = resultCreate;
                        _ = await UploadCheckpointScketches();
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/Checkpoints");

                    }
                    else
                    {
                        //suces create
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Fail", Severity.Error);
                        //NavigationManager.NavigateTo($"/");
                    }

                    break;

                case PageType.Update:
                    Checkpoint? resultUpdate = await _CheckPointServices.UpdateCheckpoint(_Checkpoint);

                    if (resultUpdate != null)
                    {
                        _ = await UploadUpdateCheckpointScketches();

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/Checkpoints");
                        //NavigationManager.NavigateTo($"/");
                    }
                    else
                    {
                        //suces create
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Fail", Severity.Error);
                        //NavigationManager.NavigateTo($"/");
                    }
                    break;
            }
        }

        private async Task<AsyncVoidMethodBuilder> UploadUpdateCheckpointScketches()
        {
            await UploadImages(tempCapturedImages, _Checkpoint.CheckpointId, true);

            return new AsyncVoidMethodBuilder();
        }

        private async Task<AsyncVoidMethodBuilder> UploadCheckpointScketches()
        {
            await UploadImages(CheckpointImages, _Checkpoint.CheckpointId, true);

            return new AsyncVoidMethodBuilder();
        }

        private async Task UploadImages(List<string> images, int Checkpoint_id, bool isPrevious)
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


                    var result = await _CheckPointServices.UploadSketchCheckpoint(content, Checkpoint_id);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Image Added to Checkpoint Item", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Image to Checkpoint Item", Severity.Error);
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

        void CheckpointUpdate(int CheckpointsId)
        {
            NavigationManager.NavigateTo($"configurationIS/Checkpoints/Update/{CheckpointsId}", forceLoad: true);
        }

        //Show Evidence 
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };
        private readonly DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;
        private int selectedEvidence = 0;

        private List<string> CheckpointImages = new List<string>();
        private readonly List<string> tempCapturedImages = new();

        private bool visibleCamera = false;
        private int imageIndex = 0;

        private CameraStreamer CameraStreamerReference;

        private string? cameraId = null;

        private int frameCount;

        private string imageData;

        private bool visibleDelete = false;
        public int removeImageIndex = 0;
        public bool isPrevious = false;
        public bool isTemporal = false;

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

                        switch (pageType)
                        {
                            case PageType.Create:
                                CheckpointImages.Add(mediaUri);
                                break;
                            case PageType.Update:
                                tempCapturedImages.Add(mediaUri);
                                break;

                        }

                    }
                }
            }
        }


        private void OpenEvidenceDialog(int index, int evidenceIndex)
        {
            photoIndex = index;
            selectedEvidence = evidenceIndex;
            visibleEvidence = true;

        }

        private void OpenCameraDialog(int index)
        {
            imageIndex = index;
            visibleCamera = true;
        }

        private async void OnRenderedHandler()
        {
            frameCount = 0;

            if (await CameraStreamerReference.GetCameraAccessAsync())
            {
                await CameraStreamerReference.ReloadAsync();

            }
        }

        private async void GetCurrentFrame()
        {
            imageData = await CameraStreamerReference.GetCurrentFrameAsync();

            if (!string.IsNullOrEmpty(imageData))
            {
                switch (pageType)
                {
                    case PageType.Create:
                        CheckpointImages.Add(imageData);
                        break;
                    case PageType.Update:
                        tempCapturedImages.Add(imageData);
                        break;

                }
            }
            visibleCamera = false;
            StateHasChanged();
            Stop();
        }

        private async void Stop()
        {
            await CameraStreamerReference.StopAsync();
        }

        private void OnFrameHandler(string _)
        {
            ++frameCount;
        }

        private void OpenDeleteDialog(int index, bool isPrev, bool isTemp)
        {
            removeImageIndex = index;
            isPrevious = isPrev;
            isTemporal = isTemp;
            visibleDelete = true;
        }
        private void RemoveImage()
        {

            if (removeImageIndex >= 0)
            {
                if (isTemporal)
                {
                    if (removeImageIndex < tempCapturedImages.Count)
                        tempCapturedImages.RemoveAt(removeImageIndex);
                }
                else
                {
                    if (removeImageIndex < CheckpointImages.Count)
                        CheckpointImages.RemoveAt(removeImageIndex);
                }
            }
            CloseDeleteModal();
        }
        void CloseDeleteModal() => visibleDelete = false;
    }
}