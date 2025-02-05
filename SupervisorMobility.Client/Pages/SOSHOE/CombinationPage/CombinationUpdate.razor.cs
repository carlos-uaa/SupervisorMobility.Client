using MudBlazor;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;


namespace SupervisorMobility.Client.Pages.SOSHOE.CombinationPage
{
    public partial class CombinationUpdate
    {
        [Parameter]
        public int? CombinationId { get; set; }

        SOSCombination _sosCombination { get; set; } = new();
        private List<SOSCombinationLogbook> mostRecentLogs = new List<SOSCombinationLogbook>();
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
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;
        public bool UpdateButton = false;

        //Edit Image
        private bool visibleEditImage = false;
        private DialogOptions dialogEditImagesOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, DisableBackdropClick = true };


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

            _sosCombination = await SOSCombinationServices.GetSOSCombination((int)CombinationId, true, true, true, true, true, true);
            if (_sosCombination.CombinationLogbooks != null)
            {
                mostRecentLogs = _sosCombination.CombinationLogbooks
                    .OrderByDescending(log => log.SOSCombinationLogbookId)
                    .Take(Math.Min(3, _sosCombination.CombinationLogbooks.Count))
                    .OrderBy(log => log.SOSCombinationLogbookId)
                    .ToList();

                logCount = mostRecentLogs.Count;
                totalLogbooks = _sosCombination.CombinationLogbooks.Count;

            }

            remainingLogs = 3 - logCount;
            logbookIds = mostRecentLogs.Select(log => log.SOSCombinationLogbookId).ToList();

            //Validacion de que se creen los tiempos en caso de que no esten

            foreach(var section in _sosCombination.SOSHub?.Sections)
            {
                if(!_sosCombination.SOSCombinationOperationSequence.Any(sc => sc.SectionId == section.SectionId))
                {
                    SOSCombinationOperationSequence newToAdd = new SOSCombinationOperationSequence();
                    newToAdd.SectionId = section.SectionId;
                    newToAdd.ProcessName = section.Step;
                    newToAdd.IsActive = true;

                    _sosCombination.SOSCombinationOperationSequence.Add(newToAdd);
                }
            }

            if (_sosCombination.Illustrations?.Any() ?? false)
            {

                foreach (var combinationImage in _sosCombination.Illustrations)
                {
                    var image = await SOSCombinationServices.ShowIlustrationSOSCombination(combinationImage.FileUploadId);
                    PreviousImages.Add((combinationImage.FileUploadId, image));
                }
            }


            cycleId = _sosCombination.SOSHub?.TrainingTime != null ? GetCycleId(_sosCombination.SOSHub.TrainingTime) : 0;

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
        //    IndexCombination = 0;
        //}

        //protected override void OnParametersSet()
        //{
        //    IndexCombination = 0;
        //}


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

        private async void DownloadExcel()
        {
            //await Exportation.ExportCombinationToExcel(CombinationId.Value);
        }

        private async Task MoveStepsProcess(int index, int direction)
        {

            int newIndex = index + direction;

            if (newIndex < 0 || newIndex >= _sosCombination.SOSCombinationOperationSequence.Count)
            {
                Console.WriteLine("Into Return move");

                return;
            }

            var StepsProcess = _sosCombination.SOSCombinationOperationSequence.ToList();
            var temp = StepsProcess[index];
            StepsProcess[index] = StepsProcess[newIndex];
            StepsProcess[newIndex] = temp;

            for (int i = 0; i < StepsProcess.Count; i++)
            {
                StepsProcess[i].SequenceId = i + 1;
            }

            _sosCombination.SOSCombinationOperationSequence = StepsProcess;
           
        }

        private async Task UpdateCombination()
        {
            Snackbar.Clear();
            UpdateButton = true;
        
            var result = await SOSCombinationServices.UpdateSOSCombination(_sosCombination);

            if (result != null)
            {
                Snackbar.Add($"Combination Updated!", Severity.Info);

                _sosCombination = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/SOSHOE/Combination");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            
            UpdateButton = false;

        }

        #region EditImage
        private UpdateCombinationImage updateImageComponent;
        public int ImageIndex;
        public bool IsPreviousPhoto;
        public int FileUploadIndex = 0;
        private List<int> OldImageRemoved = new();
        private List<(int, string)> PreviousImages = new List<(int, string)>();//id, b64 string

        private async Task OpenEditImageDialog(bool isPreviousPhoto, int index = 0, int fileUploadIndex = 0, string imageBase64 = "")
        {
            ImageIndex = index == 0 ? 1 : index;
            IsPreviousPhoto = isPreviousPhoto;
            visibleEditImage = true;
            FileUploadIndex = fileUploadIndex;

            while (updateImageComponent == null || !updateImageComponent.IsReady)
            {
                await Task.Delay(50);
            }

            if (updateImageComponent != null)
            {
                if (imageBase64 == "")
                {
                    var imagePath = "Images/CombinationSymbols/canvasImage.png";
                    imageBase64 = imagePath;
                    capturedImages.Add(imageBase64);
                }
                await updateImageComponent.LoadImageFromBase64Async(imageBase64);
            }
        }

        private void CloseEditImageDialog()
        {
            visibleEditImage = false;
            updateImageComponent = null;
        }

        public void UpdatePhoto(string updatedImage, int index, bool isPrevious)
        {
            if (isPrevious)
            {
                OldImageRemoved.Add(FileUploadIndex);
                PreviousImages.RemoveAt(index);
                FileUploadIndex = 0;
            }
            else
            {
                capturedImages.RemoveAt(index);
            }
            capturedImages.Add(updatedImage);
            CloseEditImageDialog();
            StateHasChanged();
        }

        #endregion


        #region Upload Images
        private async Task<AsyncVoidMethodBuilder> UploadEvidence()
        {
            await UploadImages();
            await UpdateRemovedFiles();

            return new AsyncVoidMethodBuilder();
        }

        private async Task UploadImages()
        {

            List<string> images = capturedImages;
            int combinationId = _sosCombination.SOSCombinationId;

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


                    var result = await SOSCombinationServices.AddIllustrationToSOSCombination(content, combinationId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Image Added to Combination", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Image to Combination", Severity.Error);
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
                    var result = await SOSCombinationServices.RemoveIlustrationFromSOSData(_sosCombination.SOSCombinationId, file);
                    if (!result)
                    {
                        Snackbar.Add($"Error removing the image", Severity.Error);
                    }
                }
            }

        }

        private void RemoveOldImage(int fileId)
        {
            OldImageRemoved.Add(fileId);
            PreviousImages.RemoveAll(p => p.Item1 == fileId);
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
    }
}