using BlazorCameraStreamer;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Department = SupervisorMobility.Client.Data.Entities.Department;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupUpdate
    {

        [Parameter]
        public int LupId { get; set; }

        private List<BreadcrumbItem> _links;

        public Lup _lup { get; set; } = new();
        public JobObservation jobObservation { get; set; } = new();

        List<Department> _departments = new();
        private int departmentID = 0;


        //Justification Modal
        private bool visible = false;
        private void OpenCancelDialog()
        {
            visible = true;
        }
        void Close() => visible = false;
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        Dictionary<int, string> imageUrls = new Dictionary<int, string>();

        bool showLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };



        protected async override Task OnInitializedAsync()
        {
            _sourceMsgLoading = new List<string>();
            for (int i = 1; i <= 11; i++)
            {
                _sourceMsgLoading.Add($"{Localizer1["Loading" + i]}");
            }


            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem("LUP", href: "/lup"),
                new BreadcrumbItem(text: Localizer["update"] + " LUP", href: "/", disabled: true),
            };

            _lup = await LupServices.GetLupByIdWhitFile(LupId);
            jobObservation = await JobObservationService.GetJobObservationById(_lup.JobObservationId, true);

            departmentID = _lup.DepartmentId != null ? (int)_lup.DepartmentId : departmentID;

            _departments = await DepartmentServices.GetDepartments();

            foreach (var evidence in _lup.Evidences)
            {
                if (evidence.ContentType == "image/png")
                {
                    var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                    imageUrls[evidence.FileUploadId] = imageUrl;
                }
            }
            showLoading = false;
        }
        private async Task EditLup()
        {
            _lup.DepartmentId = departmentID != 0 ? departmentID : _lup.DepartmentId;

            _lup.Status = 2;

            var result = await LupServices.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Updated", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        public async void CancelLup()
        {

            if (_lup.Justification == null || _lup.Justification.Length == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add the justification", Severity.Error);
                return;
            }

            _lup.DepartmentId = departmentID != 0 ? departmentID : _lup.DepartmentId;

            _lup.EndDate = DateTime.Now;
            _lup.Status = 4;

            var result = await LupServices.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Canceled", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        void GoBack()
        {
            NavigationManager.NavigateTo("/lup");
        }

        public async void FinishedLup()
        {

            _lup.EndDate = DateTime.Now;
            _lup.DepartmentId = departmentID != 0 ? departmentID : _lup.DepartmentId;

            _lup.Status = 3;

            var result = await LupServices.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Finished", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert

        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }


        //Evidence
        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string DragClass = DefaultDragClass;

        private List<FileToDisplay> fileNames = new List<FileToDisplay>();
        private List<IBrowserFile> fileNames2 = new List<IBrowserFile>();


        private bool upload = true;

        private int maxAllowedFiles = 5;
        private long maxFileSize = long.MaxValue;

        private class FileToDisplay
        {
            public string name { get; set; }
            public string ftype { get; set; }
            public string message { get; set; }
        }

        private async void RemoveEvidence(int fileUploadId)
        {
            var response = await LupServices.RemoveEvidence(LupId, fileUploadId);
            Console.WriteLine(fileUploadId);
            if (response)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Evidence removed", Severity.Info);
                _lup = await LupServices.GetLupByIdWhitFile(LupId);
                StateHasChanged();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Failed to remove evidence", Severity.Error);
            }
        }


        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            fileNames.Clear();
            fileNames2.Clear();
            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                fileNames2.Add(file);
                fileNames.Add(new FileToDisplay() { name = file.Name, ftype = file.ContentType });
            }
            Console.WriteLine($"{fileNames2.Count}");

            upload = false;

        }

        private async Task Clear()
        {
            upload = true;
            fileNames.Clear();
            fileNames2.Clear();

            ClearDragClass();
            await Task.Delay(100);
        }

        private async Task Upload()
        {
            //Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            //call function upload files
            Console.WriteLine($" en carga {fileNames2.Count}");


            foreach (var file in fileNames2)
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(
                         content: fileContent,
                         name: "\"file\"",
                         fileName: file.Name);

                var result = await FilesServices.UploadEvidences(content, LupId);
                if (result is not null)
                {
                    Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"{file.Name} Added to Lup {LupId}", Severity.Info);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Failed to upload Evidence to Lup", Severity.Error);
                    break;
                }
            }

            fileNames.Clear();
            fileNames2.Clear();
            _lup = await LupServices.GetLupByIdWhitFile(LupId);
            foreach (var evidence in _lup.Evidences)
            {
                if (evidence.ContentType == "image/png")
                {
                    var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                    imageUrls[evidence.FileUploadId] = imageUrl;
                }
            }
            StateHasChanged();

            upload = true;

        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }

        //Camera
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new List<string>();

        private bool visibleCamera = false;
        private void OpenCameraDialog()
        {
            visibleCamera = true;

        }
        void Close2() => visibleCamera = false;

        private CameraStreamer CameraStreamerReference;

        private string? cameraId = null;

        private int frameCount;

        private string imageData;

        private async void OnRenderedHandler()
        {
            frameCount = 0;

            // Check camera-access or ask user, if it's not allowed currently
            if (await CameraStreamerReference.GetCameraAccessAsync())
            {
                // Reloading re-initializes the stream and starts the
                // stream automatically if the Autostart parameter is set
                await CameraStreamerReference.ReloadAsync();

                // If Autostart is not set, you have to manually start the stream again
                /* await CameraStreamerReference.StartAsync(); */
            }
        }

        private async void Start()
        {
            await CameraStreamerReference.StartAsync();
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
                capturedImages.Add(imageData);
            }
            visibleCamera = false;
            StateHasChanged();
            Stop();
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

        private async Task UploadEvidence()
        {
            if (capturedImages.Count > 0)
            {
                foreach (var imageData in capturedImages)
                {
                    if (!string.IsNullOrEmpty(imageData))
                    {
                        // Elimina la cabecera si está presente
                        var base64Data = imageData.Replace("data:image/png;base64,", "");

                        if (IsValidBase64String(base64Data))
                        {
                            // Convierte base64Data en bytes
                            var imageBytes = Convert.FromBase64String(base64Data);

                            using var content = new MultipartFormDataContent();
                            var imageStream = new MemoryStream(imageBytes);
                            var fileContent = new StreamContent(imageStream);
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                            content.Add(
                                content: fileContent,
                                name: "\"file\"",
                                fileName: "evidence.png");

                            // Llama a tu servicio de carga de archivos aquí
                            var result = await FilesServices.UploadEvidences(content, LupId);

                            if (result is not null)
                            {
                                Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add("Image Added to Lup", Severity.Info);
                                _lup = await LupServices.GetLupByIdWhitFile(LupId);


                                foreach (var evidence in _lup.Evidences)
                                {
                                    if (evidence.ContentType == "image/png")
                                    {
                                        var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                                        imageUrls[evidence.FileUploadId] = imageUrl;
                                    }
                                }

                                StateHasChanged();
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add("Failed to upload Image to Lup", Severity.Error);
                            }
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add("Invalid image data", Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("No image data to upload", Severity.Warning);
                    }
                }

                // Limpia la lista de imágenes capturadas después de cargarlas
                capturedImages.Clear();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("No images to upload", Severity.Warning);
            }
        }

        private void RemoveImage(int index)
        {
            if (index >= 0 && index < capturedImages.Count)
            {
                capturedImages.RemoveAt(index);
            }
        }


        //Show Photo
        private DialogOptions dialogPhotoOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true};

        private bool visiblePhoto = false;

        private int photoIndex = 0;

        private void OpenPhotoDialog(int index)
        {
            photoIndex = index;
            visiblePhoto = true;

        }

        private void CloseChip() { }

    }
}
