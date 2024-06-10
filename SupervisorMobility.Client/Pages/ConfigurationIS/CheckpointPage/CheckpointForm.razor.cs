using BlazorCameraStreamer;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.IS;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Policy;


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

        //imgs
        private List<ImgDto> CheckpointImages = new List<ImgDto>();
        private readonly List<string> tempCapturedImages = new();
        class ImgDto
        {
            public string src { get; set; }
            public int imgId { get; set; }
        }
        //standars img dictinary
        private Dictionary<int, List<ImgDto>> StandarImages = new Dictionary<int, List<ImgDto>>();
        private Dictionary<int, List<string>> tempStandarImages = new Dictionary<int, List<string>>();

        bool seeImgManager { get; set; } = false;

        //PageType
        public enum PageType
        {
            Details,
            Create,
            Update,
            Another
        }
        public PageType pageType { get; set; }
        //Flags
        private bool AddNormStandar = false;

        //Spectable
        private string searchStringNormStandar = "";
        private CheckpointNorm selectedItem1 = null;
        private CheckpointNorm elementBeforeEdit;
        private HashSet<CheckpointNorm> selectedItems1 = new HashSet<CheckpointNorm>();
        private TableApplyButtonPosition applyButtonPosition = TableApplyButtonPosition.Start;
        private TableEditButtonPosition editButtonPosition = TableEditButtonPosition.Start;
        private TableEditTrigger editTrigger = TableEditTrigger.RowClick;

        private IEnumerable<CheckpointNorm> Elements = new List<CheckpointNorm>();
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

        private int selectedRowNumber = -1;
        private MudTable<CheckpointNorm> SelectTableEvent;
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
                                _Checkpoint = await _CheckPointServices.GetCheckpoint((int)CheckpointId, true, true);
                                _links.Add(new BreadcrumbItem(text: _Checkpoint.CheckpointTitle, href: $"/configurationIS/Checkpoint/Details/{CheckpointId}"));
                                _links.Add(new BreadcrumbItem(text: Localizer["Details"], href: $"/configurationIS/Checkpoint/{CheckpointId}", disabled: true));
                            }
                            break;
                        case PageType.Create:
                            _links.Add(new BreadcrumbItem(text: Localizer["Create"], href: $"/configurationIS/Checkpoint/", disabled: true));
                            _Checkpoint.IsActive = true;
                            break;

                        case PageType.Update:
                            if (CheckpointId != null)
                            {
                                _Checkpoint = await _CheckPointServices.GetCheckpoint((int)CheckpointId, true, true, true);
                                //_Checkpoint 
                                _links.Add(new BreadcrumbItem(text: _Checkpoint.CheckpointTitle, href: $"/configurationIS/Checkpoint/Details/{CheckpointId}"));
                                _links.Add(new BreadcrumbItem(text: Localizer["Update"], href: $"/configurationIS/Checkpoint/", disabled: true));
                            }
                            break;
                    }

                    if (pageType != PageType.Create)
                    {
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

                        if (_Checkpoint.Standars != null && _Checkpoint.Standars.Count > 0)
                        {
                            foreach (var standar in _Checkpoint.Standars)
                            {

                                List<ImgDto> Stdimg = new List<ImgDto>();

                                if (standar.Sketches != null && standar.Sketches.Count > 0)
                                {
                                    foreach (var standarSketch in standar.Sketches)
                                    {

                                        var imageUrl = await _CheckPointServices.ShowImageCheckpointNorm(standarSketch.FileUploadId);
                                        ImgDto tmpimgDto = new();
                                        tmpimgDto.src = imageUrl;
                                        tmpimgDto.imgId = standarSketch.FileUploadId;

                                        Stdimg.Add(tmpimgDto);
                                    }
                                }

                                StandarImages.Add(standar.CheckpointNormId, Stdimg);

                                List<string> tmpImg = new List<string>();

                                tempStandarImages.Add( standar.CheckpointNormId, tmpImg );

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
                        NavigationManager.NavigateTo($"/configurationIS/Checkpoint");

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
                        NavigationManager.NavigateTo($"/configurationIS/Checkpoint");
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

            //Upload images from standars
            foreach(var standar in _Checkpoint.Standars) {

                if (tempStandarImages.TryGetValue(standar.CheckpointNormId,out var newImgs) )
                {
                    if(newImgs != null)
                    {
                        await UploadCheckpointNormScketches(newImgs, standar.CheckpointNormId);
                    }
                }
            }

            return new AsyncVoidMethodBuilder();
        }

        private async Task<AsyncVoidMethodBuilder> UploadCheckpointScketches()
        {
            await UploadImages(CheckpointImages.Select(img => img.src).ToList(), _Checkpoint.CheckpointId, true);

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

                    content.Add(fileContent, "\"file\"", "Sketch.png");


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

        private async Task<AsyncVoidMethodBuilder> UploadCheckpointNormScketches(List<string> images, int Checkpoint_id)
        {
            await UploadNormImages(images, Checkpoint_id);

            return new AsyncVoidMethodBuilder();
        }

        private async Task UploadNormImages(List<string> images, int Checkpoint_Norm_id)
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

                    content.Add(fileContent, "\"file\"", "Sketch.png");


                    var result = await _CheckPointServices.UploadSketchCheckpointNorm(content, Checkpoint_Norm_id);

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
            NavigationManager.NavigateTo($"configurationIS/Checkpoint/Update/{CheckpointsId}", forceLoad: true);
        }

        
        private async Task UploadFiles(InputFileChangeEventArgs e, int type, int id_stdCheckpoint=0)
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
                                ImgDto _tmpimg = new ImgDto();
                                _tmpimg.src = mediaUri;
                                if (id_stdCheckpoint != 0)
                                {
                                    StandarImages[id_stdCheckpoint].Add(_tmpimg);
                                }
                                else
                                {
                                CheckpointImages.Add(_tmpimg);
                                }
                                break;
                            case PageType.Update:
                                if (id_stdCheckpoint != 0)
                                {
                                    tempStandarImages[id_stdCheckpoint].Add(mediaUri);
                                }
                                else
                                {
                                    tempCapturedImages.Add(mediaUri);
                                }
                                break;

                        }

                    }
                }
            }
        }


        private void OpenSketchDialog(int index, int SketchIndex)
        {
            photoIndex = index;
            selectedSketch = SketchIndex;
            visibleSketch = true;

        }

        private void OpenCameraDialog(int index, int Std_id = 0)
        {
            ckStd_id = Std_id;
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
                        if(ckStd_id != 0)
                        {
                            tempStandarImages[ckStd_id].Add(imageData);
                        }
                        else
                        {
                            tempCapturedImages.Add(imageData);
                        }
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
                        if(removeImageId != 0)
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


        // Create Standars
        void CreateStandars(int chekpointId)
        {
            NavigationManager.NavigateTo($"/configurationIS/Checkpoint/Details/{chekpointId}/CreateStandars");
        }
        //re order Standars
        void StandarsSequence(int chekpointId)
        {
            NavigationManager.NavigateTo($"/configurationIS/Checkpoint/Details/{chekpointId}/sequence");
        }
        // Delete question
        async Task DeleteStandars(int chekpointId, int chPointNorm_Id)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this Standar/Norm?");

            if (confirm)
            {
                _Checkpoint.Standars.ToList().RemoveAll(st => st.CheckpointNormId == chPointNorm_Id);
                await _CheckPointServices.DeleteCheckpointNorm(chPointNorm_Id);
            }
        }

        // Update question
        void UpdateStandars(int chekpointId, int StandarsId)
        {
            NavigationManager.NavigateTo($"/configurationIS/Checkpoint/Details/{chekpointId}/UpdateStandars/{StandarsId}");
        }  
        void DetailsStandars(int chekpointId, int StandarsId)
        {
            NavigationManager.NavigateTo($"/configurationIS/Checkpoint/Details/{chekpointId}/DetailsStandars/{StandarsId}");
        }

        private string searchString = "";

        private bool FilterFunc(CheckpointNorm element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            //if (element.DataStandars.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (element.ItemOrder.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (element.DataPanelStandarsId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if ($"{element.DataPanelStandarsId}".Contains(searchString))
            //    return true;


            //if (element.DataStandars.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
            //    return true;

            //var searchWords = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //// Verificar si todas las palabras en searchWords están contenidas en Data Standars
            //if (searchWords.All(word => element.DataStandars.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
            //    return true;

            return false;
        }

       

        private void RowClickEvent(TableRowClickEventArgs<CheckpointNorm> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(CheckpointNorm element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                DetailsStandars((int)CheckpointId, element.CheckpointNormId);
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

      

        private void AddOneMoreNormStandar()
        {
            AddNormStandar = true;
            if (_Checkpoint.Standars == null)
            {
                _Checkpoint.Standars = new List<CheckpointNorm>();
            }


            Random random = new Random();
            int positiveRandomNumber = random.Next(1, int.MaxValue); // Genera un número aleatorio positivo
            int negativeRandomNumber = -positiveRandomNumber;

            CheckpointNorm NewItemSpec = new CheckpointNorm();
          
            StandarImages.Add(negativeRandomNumber, new List<ImgDto>()); 
            tempStandarImages.Add(negativeRandomNumber, new List<string>()); 

            NewItemSpec.CheckpointNormId = negativeRandomNumber; 
            NewItemSpec.Standard = "New Item, pls change this";
            NewItemSpec.IsActive = true;

            _Checkpoint.Standars?.Add(NewItemSpec);


            AddNormStandar = false;
        }

        private bool FilterFuncNormStandar(CheckpointNorm element)
        {

            if (string.IsNullOrWhiteSpace(searchString))
                return true;


            return false;
        }

        private void BackupItem(object element)
        {
            elementBeforeEdit = new()
            {
                CheckpointNormId = ((CheckpointNorm)element).CheckpointNormId,
                Standard = ((CheckpointNorm)element).Standard,
                ItemOrder = ((CheckpointNorm)element).ItemOrder,
                IsActive = ((CheckpointNorm)element).IsActive
            };

        }

        private void ResetItemToOriginalValues(object element)
        {
            ((CheckpointNorm)element).CheckpointNormId = elementBeforeEdit.CheckpointNormId;
            ((CheckpointNorm)element).Standard = elementBeforeEdit.Standard;
            ((CheckpointNorm)element).ItemOrder = elementBeforeEdit.ItemOrder;
            ((CheckpointNorm)element).IsActive = elementBeforeEdit.IsActive;
        }

        private void ItemHasBeenCommitted(object element)
        {
            Console.WriteLine("Comitt item");
        }

        int ckStd_id { get; set; } = 0;
        void OpenImagesEditor(int Check_iD)
        {
            ckStd_id = Check_iD;
            seeImgManager = true; 
        }

        void CloseImagesEditor() => seeImgManager = false;

    } // partial class end

}