using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using BlazorCameraStreamer;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;

namespace SupervisorMobility.Client.Pages.Inicio.KaizenPage
{
    public partial class UpdateKaizen
    {
        [Parameter]
        public int KaizenId { get; set; }
        Kaizen _kaizen = new();
        readonly DateTime minDate = DateTime.Now.AddDays(-1);

        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        List<Plant> _plants = new();
        List<Area> _areas = new();
        int plantId = 0;
        int areaId = 0;

        List<User> _seniorSupervisors = new();
        List<User> _allSSVs = new();
        List<User> _supervisors = new();
        List<User> _operators = new();
        List<User> operatorUsers = new();

        int ssvId = 0;
        int supervisorId = 0;
        int operatorId = 0;

        string ssvName = "";
        string svName = "";
        string operatorName = "";

        //Datetime
        public string hour1 = "";
        readonly TimeSpan? startHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;
        public class ItemModel
        {
            public int KaizenId { get; set; }
            public int EffectId { get; set; }
            public string Benefit { get; set; }
            public bool IsActive { get; set; }
        }

        List<ItemModel> items = new ();
        List<ItemModel> tempItems = new();
        bool showLoading = true;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["kaizen"], href: "/kaizen"),
                    new BreadcrumbItem(text: Localizer["update kaizen"], href: "", disabled: true)
                };
            AddItem();
            BreadcrumbService.UpdateBreadcrumbs(_links);
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
               await GetKaizen();
                showLoading = false;
            }


        }

        public async Task GetKaizen()
        {
            capturedImages = new();
            capturedImagesThen = new();
            _kaizen = await KaizenServices.GetKaizenById(KaizenId, true, true, true, true);


            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();

            if (user.UserType == 2)
            {
                plantId = user.PlantId ?? 0;
                areaId = 0;

                if(user.Areas != null)
                {
                    _areas = user.Areas.ToList();
                }

                ssvId = user.UserId;
                if (user.Subordinates != null)
                {
                    foreach (var sv in user.Subordinates.ToList())
                    {
                        _supervisors.Add(sv);
                    }
                }


            }
            else if (user.UserType == 3)
            {
                plantId = user.PlantId ?? 0;
                areaId = user.Areas != null && user.Areas.Count > 0 ? user.Areas.FirstOrDefault().AreaId : 0;

                _areas = await AreaServices.GetAreas(plantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();

                ssvId = user.SuperiorId ?? 0;
                supervisorId = user.UserId;

                svName = user.Name;
                
                if (user.Superior != null)
                {
                    ssvName = user.Superior.Name;
                    _seniorSupervisors.Add(user.Superior);
                }

                _supervisors.Add(user);
                
                if(user.Subordinates != null)
                {
                    foreach (var op in user.Subordinates.ToList())
                    {
                        operatorUsers.Add(op);
                    }
                }
            }


            plantId = _kaizen.PlantId != null ? (int)_kaizen.PlantId : 0;
            areaId = _kaizen.AreaId != null ? (int)_kaizen.AreaId : 0;
            ssvId = _kaizen.SeniorSupervisorId != null ? (int)_kaizen.SeniorSupervisorId : 0;
            supervisorId = _kaizen.SupervisorId != null ? (int)_kaizen.SupervisorId : 0;
            operatorId = _kaizen.ProposedId != null ? (int)_kaizen.ProposedId : 0;
            ssvName = _kaizen.SeniorSupervisor != null ? _kaizen.SeniorSupervisor.Name : "";
            svName = _kaizen.Supervisor != null ? _kaizen.Supervisor.Name : "";
            operatorName = _kaizen.Proposed != null ? _kaizen.Proposed.Name : "";

            if (plantId != 0)
            {
                _areas = await AreaServices.GetAreas(plantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();
            }
            if (areaId != 0)
            {
                _seniorSupervisors = new();
                _allSSVs = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _allSSVs = _allSSVs.OrderBy(s => s.Name).ToList();

                foreach (User ssv in _allSSVs)
                {
                    if (ssv.Areas?.ToList().FindIndex(a => a.AreaId == areaId) != -1)
                    {
                        _seniorSupervisors.Add(ssv);
                    }
                }
            }
            if (ssvId != 0)
            {
                _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 3, false, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            }

            if (supervisorId != 0)
            {
                if (user.UserType == 1 || user.UserType == 2)
                {
                    _operators = await UsersService.GetSubordinates(supervisorId, false);
                    _operators = _operators.OrderBy(o => o.Name).ToList();
                    foreach (var operatorUser in _operators)
                    {
                        if (operatorUser.Areas.Any(a => a.AreaId == areaId) && operatorUser.SuperiorId == supervisorId)
                        {
                            operatorUsers.Add(operatorUser);
                        }
                    }
                }
            }

            if (_kaizen.PreviousEvidences != null && _kaizen.PreviousEvidences.Count > 0)
            {
                foreach (var evidence in _kaizen.PreviousEvidences)
                {
                    var imageUrl = await FilesServices.ShowImagePreviousEvidence(evidence.FileUploadId);
                    capturedImages.Add(imageUrl);
                }
            }

            if (_kaizen.ThenEvidences != null && _kaizen.ThenEvidences.Count > 0)
            {
                foreach (var evidence in _kaizen.ThenEvidences)
                {
                    var imageUrl = await FilesServices.ShowImageThenEvidence(evidence.FileUploadId);
                    capturedImagesThen.Add(imageUrl);

                }
            }

            items = new();
            tempItems = new();

            if (_kaizen.Transactions != null)
            {
                foreach (var transaction in _kaizen.Transactions)
                {
                    var item = new ItemModel
                    {
                        KaizenId = transaction.KaizenTransactionId ?? 0,
                        EffectId = int.TryParse(transaction.Title, out var title) ? title : 0,
                        Benefit = transaction.Description,
                        IsActive = transaction.IsActive ?? false
                    };
                    items.Add(item);
                }
            }

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


        private async void ShowAreas()
        {
            supervisorId = 0;
            ssvId = 0;
            operatorId = 0;
            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();
        }

        private async void ShowOperators()
        {
            if (supervisorId != 0)
            {
                svName = _supervisors.FirstOrDefault(sv => sv.UserId == supervisorId, new User()).Name;
            }
            if (user.UserType == 1 || user.UserType == 2)
            {
                _operators = await UsersService.GetSubordinates(supervisorId, false);
                _operators = _operators.OrderBy(o => o.Name).ToList();
            }
            operatorUsers = new();
            operatorId = 0;
            //operator User
            foreach (var operatorUser in _operators)
            {
                if (operatorUser.Areas.Any(a => a.AreaId == areaId) && operatorUser.SuperiorId == supervisorId)
                {
                    operatorUsers.Add(operatorUser);
                }
            }
            StateHasChanged();
        }

        private async void ShowSeniorSupervisors()
        {
            supervisorId = 0;
            ssvId = 0;
            operatorId = 0;
            _seniorSupervisors = new();
            _allSSVs = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
            _allSSVs = _allSSVs.OrderBy(s => s.Name).ToList();

            foreach (User ssv in _allSSVs)
            {
                if (ssv.Areas?.ToList().FindIndex(a => a.AreaId == areaId) != -1)
                {
                    _seniorSupervisors.Add(ssv);
                }
            }
            StateHasChanged();
        }

        private async void ShowSupervisors()
        {
            if (ssvId != 0)
                ssvName = _seniorSupervisors.FirstOrDefault(ssv => ssv.UserId == ssvId, new User()).Name;

            supervisorId = 0;
            operatorId = 0;
            _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 3, false, false);
            _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            StateHasChanged();
        }

        private void OperatorName()
        {
            if (operatorId != 0 && operatorUsers != null)
                operatorName = operatorUsers.FirstOrDefault(op => op.UserId == operatorId, new User()).Name;

        }


        private async Task EditKaizen()
        {

            _kaizen.PlantId = plantId;
            _kaizen.AreaId = areaId;

            _kaizen.SeniorSupervisorId = ssvId;
            _kaizen.SupervisorId = supervisorId;
            _kaizen.ProposedId = operatorId;
            _kaizen.Status = 2;


            FormatDate();
            GenerateKaizenTransaction();

            _ = await UploadEvidence();
            var result = await KaizenServices.UpdateKaizen(_kaizen);

            if (result)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Kaizen Updated", Severity.Info);
                NavigationManager.NavigateTo("/kaizen");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

        }

        public void GenerateKaizenTransaction()
        {
            _kaizen.Transactions?.Clear();
            foreach (var item in items)
            {
                var kaizenTransaction = new KaizenTransaction
                {
                    KaizenTransactionId = item.KaizenId,
                    Title = item.EffectId.ToString(),
                    Description = item.Benefit,
                    Type = 1,
                    IsActive = item.IsActive
                };

                _kaizen.Transactions?.Add(kaizenTransaction);
            }

            foreach (var item in tempItems)
            {
                var kaizenTransaction = new KaizenTransaction
                {
                    KaizenTransactionId = 0,
                    Title = item.EffectId.ToString(),
                    Description = item.Benefit,
                    Type = 1,
                    IsActive = true
                };

                _kaizen.Transactions?.Add(kaizenTransaction);
            }

        }

        public void FormatDate()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _kaizen.CreateDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                _kaizen.CreateDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);


                hour1 = _kaizen.CreateDate?.ToShortDateString() + $" {startHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour1);



                _kaizen.CreateDate = newDate1;
            }
            else
            {
                hour1 = _kaizen.CreateDate?.ToShortDateString() + $" {startHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                    _kaizen.CreateDate = newDate1;
            }
        }

        //Camera
        private readonly DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new();
        private List<string> capturedImagesThen = new();


        private readonly List<string> tempCapturedImages = new();
        private readonly List<string> tempCapturedImagesThen = new();

        private bool visibleCamera = false;
        private int imageIndex = 0;

        private void OpenCameraDialog(int index)
        {
            imageIndex = index;
            visibleCamera = true;

        }

        private CameraStreamer CameraStreamerReference;

        private readonly string? cameraId = null;

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
                    tempCapturedImages.Add(imageData);
                }
                else
                {
                    tempCapturedImagesThen.Add(imageData);
                }
            }
            visibleCamera = false;
            StateHasChanged();
            Stop();
        }

        private async Task UpdateKaizenEvidence()
        {
            capturedImages = new();
            capturedImagesThen = new();
            Kaizen kaizenTemp = await KaizenServices.GetKaizenById(KaizenId, true, true, true, true);

            if (kaizenTemp.PreviousEvidences != null && kaizenTemp.PreviousEvidences.Count > 0)
            {
                foreach (var evidence in kaizenTemp.PreviousEvidences)
                {
                    var imageUrl = await FilesServices.ShowImagePreviousEvidence(evidence.FileUploadId);
                    capturedImages.Add(imageUrl);

                }
            }
            if (kaizenTemp.ThenEvidences != null && kaizenTemp.ThenEvidences.Count > 0)
            {
                foreach (var evidence in kaizenTemp.ThenEvidences)
                {
                    var imageUrl = await FilesServices.ShowImageThenEvidence(evidence.FileUploadId);
                    Console.WriteLine(evidence.FileUploadId);
                    capturedImagesThen.Add(imageUrl);

                }
            }

            _kaizen.PreviousEvidences = kaizenTemp.PreviousEvidences;
            _kaizen.ThenEvidences = kaizenTemp.ThenEvidences;

            StateHasChanged();
        }


        //Delete Kaizen evidence
        private bool visibleDelete = false;
        public int removeImageIndex = 0;
        public bool isPrevious = false;
        public bool isTemporal = false;


        private void OpenDeleteDialog(int index, bool isPrev, bool isTemp)
        {
            removeImageIndex = index;
            isPrevious = isPrev;
            isTemporal = isTemp;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private readonly DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


        private async void RemoveImage()
        {
            if (isTemporal)
            {
                if (removeImageIndex >= 0 && removeImageIndex < tempCapturedImages.Count || removeImageIndex < tempCapturedImagesThen.Count)
                {
                    if (isPrevious)
                    {
                        tempCapturedImages.RemoveAt(removeImageIndex);
                    }
                    else
                    {
                        tempCapturedImagesThen.RemoveAt(removeImageIndex);
                    }
                }
            }
            else if(removeImageIndex >= 0 && removeImageIndex < capturedImages.Count || removeImageIndex < capturedImagesThen.Count)
            {
                if (isPrevious && _kaizen.PreviousEvidences != null)
                {
                    var evidence = _kaizen.PreviousEvidences.ElementAtOrDefault(removeImageIndex);
                    if (evidence != null)
                    {
                        await RemoveEvidence(evidence.FileUploadId, true);
                        
                    }
                }
                else if(_kaizen.ThenEvidences != null)
                {
                    var evidence = _kaizen.ThenEvidences.ElementAtOrDefault(removeImageIndex);
                    if (evidence != null)
                    {
                        await RemoveEvidence(evidence.FileUploadId, false);

                    }
                }
            }

            removeImageIndex = 0;
            isPrevious = false;
            isTemporal = false;
            visibleDelete = false;

            StateHasChanged();
        }



        private static bool IsValidBase64String(string base64String)
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


                    var result = isPrevious
                     ? await FilesServices.UploadEvidencesKaizenPrevious(content, kaizenId)
                     : await FilesServices.UploadEvidencesKaizenThen(content, kaizenId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Image Added to Kaizen", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Image to Kaizen", Severity.Error);
                    }

                }

                images.Clear();
            }

        }

        private async Task<AsyncVoidMethodBuilder> UploadEvidence()
        {
            await UploadImages(tempCapturedImages, _kaizen.KaizenId, true);
            await UploadImages(tempCapturedImagesThen, _kaizen.KaizenId, false);

            return new AsyncVoidMethodBuilder();
        }

        private async Task UploadFiles(InputFileChangeEventArgs e, int type)
        {
            foreach (var file in e.GetMultipleFiles())
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    using Stream mediaStream = file.OpenReadStream(file.Size);
                    MemoryStream ms = new();
                    await mediaStream.CopyToAsync(ms);
                    string mediaUri = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

                    if (type == 1)
                    {
                        tempCapturedImages.Add(mediaUri);
                    }
                    else
                    {
                        tempCapturedImagesThen.Add(mediaUri);
                    }
                }
            }
        }

        private async Task RemoveEvidence(int fileUploadId, bool isPreviousEvidence)
        {
            var response = await KaizenServices.RemoveEvidence(KaizenId, fileUploadId, isPreviousEvidence);
            if (response)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Evidence removed", Severity.Info);
                await UpdateKaizenEvidence();
                StateHasChanged();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Failed to remove evidence", Severity.Error);
            }
        }


        void AddItem()
        {
            tempItems.Add(new ItemModel());
        }

        void RemoveItem(ItemModel item, bool isTemp)
        {
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

        private bool visibleSign = false;
        private int userTypeSign = 0;
        private void OpenSignComment(int userType)
        {
            visibleSign = true;
            userTypeSign = userType;
        }
        void CloseSign() => visibleSign = false;
        private readonly DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

        public void SignDate()
        {
            if (userTypeSign == 1)
            {
                _kaizen.IsSignedSSV = true;
                Console.WriteLine("SSV");
            }
            else
            {
                _kaizen.IsSignedSupervisor = true;
                Console.WriteLine("SV");
            }
            userTypeSign = 0;
            visibleSign = false;
        }

        //Show Evidence 
        private readonly DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;
        private int selectedEvidence = 0;
        private void OpenEvidenceDialog(int index, int evidenceIndex)
        {
            photoIndex = index;
            selectedEvidence = evidenceIndex;
            visibleEvidence = true;

        }


    }
}