using BlazorCameraStreamer;
using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.ConfigurationIS.CheckpointPage.CheckpointNormPage
{
    public partial class CheckpointNormForm
    {
        [Parameter]
        public int CheckpointId { get; set; }
        [Parameter]
        public int? Norm_Id { get; set; }

        public CheckpointNorm _CheckpointNorm { get; set; } = new CheckpointNorm();
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


        //PageType
        public enum PageType
        {
            Details,
            Create,
            Update,
            Another
        }
        public PageType pageType { get; set; }
        //imgs
        private List<ImgDto> CheckpointImages = new List<ImgDto>();
        private readonly List<string> tempCapturedImages = new();
        class ImgDto
        {
            public string src { get; set; }
            public int imgId { get; set; }
        }
        //Show Sketch 
        private DialogOptions dialogSketchOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };
        private readonly DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        private bool visibleSketch = false;

        private int photoIndex = 0;
        private int selectedSketch = 0;

        private bool visibleCamera = false;
        private int imageIndex = 0;

        private CameraStreamer CameraStreamerReference;

        private string? cameraId = null;

        private int frameCount;

        private string imageData;

        private bool visibleDelete = false;
        public int removeImageIndex = 0;
        public int removeImageId = 0;
        public bool isPrevious = false;
        public bool isTemporal = false;


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavigationManager.Uri;

            pageType = currentUrl.Contains("DetailsStandars", StringComparison.OrdinalIgnoreCase) ? PageType.Details : PageType.Another;

            if (pageType == PageType.Another)
            {
                pageType = currentUrl.Contains("CreateStandars", StringComparison.OrdinalIgnoreCase) ? PageType.Create : PageType.Another;
            }

            if (pageType == PageType.Another)
            {
                pageType = currentUrl.Contains("UpdateStandars", StringComparison.OrdinalIgnoreCase) ? PageType.Update : PageType.Another;
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
                new BreadcrumbItem(text: Localizer["Checkpoints"], href: "/configurationIS/Checkpoint" )
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
                                _CheckpointNorm = await _CheckPointServices.GetCheckpointNorm((int)Norm_Id, true);
                                _Checkpoint = _CheckpointNorm.Checkpoint;
                                _links.Add(new BreadcrumbItem(text: Localizer["Details"], href: $"/configurationIS/Checkpoint/{CheckpointId}"));
                                _links.Add(new BreadcrumbItem(text: _CheckpointNorm.Checkpoint?.CheckpointTitle, href: $"/configurationIS/Checkpoint/Details/{CheckpointId}"));
                                _links.Add(new BreadcrumbItem(text: Localizer["DetailsStandar"], href: $"/configurationIS/Checkpoint/{CheckpointId}"));
                                _links.Add(new BreadcrumbItem(text: _CheckpointNorm.Standard, href: $"/configurationIS/Checkpoint/Details/{CheckpointId}/DetailsStandars", disabled: true));
                            }
                            break;
                        case PageType.Create:
                            _Checkpoint = await _CheckPointServices.GetCheckpoint((int)CheckpointId);
                            _links.Add(new BreadcrumbItem(text: Localizer["Details"], href: $"/configurationIS/Checkpoint/{CheckpointId}"));
                            _links.Add(new BreadcrumbItem(text: _CheckpointNorm.Checkpoint?.CheckpointTitle, href: $"/configurationIS/Checkpoint/Details/{CheckpointId}"));
                            _links.Add(new BreadcrumbItem(text: Localizer["CreateStandar"], href: $"/configurationIS/Checkpoint/{CheckpointId}", disabled: false));
                            _CheckpointNorm.IsActive = true;
                            break;

                        case PageType.Update:
                            if (CheckpointId != null)
                            {
                                _Checkpoint = await _CheckPointServices.GetCheckpoint((int)CheckpointId, false);
                                //_Checkpoint
                                _links.Add(new BreadcrumbItem(text: _Checkpoint.CheckpointTitle, href: $"/configurationIS/Checkpoint/Details/{CheckpointId}"));
                                _links.Add(new BreadcrumbItem(text: Localizer["Update"], href: $"/configurationIS/Checkpoint/", disabled: true));
                            }
                            break;
                    }

                    if (_Checkpoint.Sketches != null && _Checkpoint.Sketches.Count > 0)
                    {
                        foreach (var Sketch in _Checkpoint.Sketches)
                        {
                            var imageUrl = await _CheckPointServices.ShowImageCheckpoint(Sketch.FileUploadId);
                            ImgDto tmpimgDto = new();
                            tmpimgDto.src = imageUrl;
                            tmpimgDto.imgId = Sketch.FileUploadId;

                            CheckpointImages.Add(tmpimgDto);
                        }
                    }

                    // if (pageType != PageType.Create)
                    // {
                    //     if (_Checkpoint.Sketches != null && _Checkpoint.Sketches.Count > 0)
                    //     {
                    //         foreach (var Sketch in _Checkpoint.Sketches)
                    //         {
                    //             var imageUrl = await _CheckPointServices.ShowImageCheckpoint(Sketch.FileUploadId);
                    //             ImgDto tmpimgDto = new();
                    //             tmpimgDto.src = imageUrl;
                    //             tmpimgDto.imgId = Sketch.FileUploadId;

                    //             CheckpointImages.Add(tmpimgDto);
                    //         }
                    //     }

                    //     if (_Checkpoint.Standars != null && _Checkpoint.Standars.Count > 0)
                    //     {
                    //         foreach (var standar in _Checkpoint.Standars)
                    //         {

                    //             List<ImgDto> Stdimg = new List<ImgDto>();

                    //             if (standar.Sketches != null && standar.Sketches.Count > 0)
                    //             {
                    //                 foreach (var standarSketch in standar.Sketches)
                    //                 {

                    //                     var imageUrl = await _CheckPointServices.ShowImageCheckpointNorm(standarSketch.FileUploadId);
                    //                     ImgDto tmpimgDto = new();
                    //                     tmpimgDto.src = imageUrl;
                    //                     tmpimgDto.imgId = standarSketch.FileUploadId;

                    //                     Stdimg.Add(tmpimgDto);
                    //                 }
                    //             }

                    //             StandarImages.Add(standar.CheckpointNormId, Stdimg);

                    //             List<string> tmpImg = new List<string>();

                    //             tempStandarImages.Add(standar.CheckpointNormId, tmpImg);

                    //         }
                    //     }
                    // }

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


        private void OpenSketchDialog(int index, int SketchIndex)
        {
            photoIndex = index;
            selectedSketch = SketchIndex;
            visibleSketch = true;

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
                        ImgDto _tmpimg = new ImgDto();
                        _tmpimg.src = imageData;
                        CheckpointImages.Add(_tmpimg);
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

        private void OpenDeleteDialog(int index, bool isPrev, bool isTemp, int id = 0)
        {
            removeImageIndex = index;
            isPrevious = isPrev;
            isTemporal = isTemp;
            removeImageId = id;
            visibleDelete = true;
        }
        private void RemoveImage()
        {

            if (removeImageIndex >= 0)
            {
                if (isTemporal)
                {
                    if (removeImageIndex < tempCapturedImages.Count)
                    {
                        tempCapturedImages.RemoveAt(removeImageIndex);
                    }
                }
                else
                {
                    if (removeImageIndex < CheckpointImages.Count)
                    {
                        if (removeImageId != 0)
                            RemoveSketch(removeImageId);

                        CheckpointImages.RemoveAt(removeImageIndex);
                        //llamar funcion para eliminar imagen
                        visibleDelete = false;

                    }
                }
            }
            CloseDeleteModal();

        }
        void CloseDeleteModal() => visibleDelete = false;

        private async Task RemoveSketch(int fileUploadId)
        {
            var response = await _CheckPointServices.RemoveSketchCheckPoint((int)CheckpointId, fileUploadId);
            if (response)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Sketch removed", Severity.Info);
                StateHasChanged();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Failed to remove Sketch", Severity.Error);
            }
        }



        /////
    }
}