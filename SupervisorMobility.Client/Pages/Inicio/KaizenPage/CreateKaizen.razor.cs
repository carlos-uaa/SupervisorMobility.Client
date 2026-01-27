using BlazorCameraStreamer;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.Inicio.KaizenPage
{
    public partial class CreateKaizen
    {
        Kaizen _kaizen { get; set; } = new();
        DateTime minDate = DateTime.Now.AddDays(-1);

        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        int plantId = 0;
        int areaId = 0;

        List<User> _seniorSupervisors { get; set; } = new();
        List<User> _allSSVs { get; set; } = new();
        List<User> _supervisors { get; set; } = new();
        List<User> _operators = new();
        public List<User> operatorUsers = new();
        int ssvId = 0;
        int supervisorId = 0;
        int operatorId = 0;
        int effectId = 0;

        string costText = "Ahorro de $133.47 por hora por reparación de paneles con marca, se reparan 4 paneles por hora, total de paneles 900.  Total de horas = (900 / 4) = 225 horas   Total de ahorro = (225 * 133.47) = $30.030.75";
        string laborText = "Costo por modificación de troquel $133.47 por hora, se usaron 18 horas, total: $2,402.46";
        string materialText = "N/A";
        string machineText = "N/A";


        string ssvName = "";
        string svName = "";
        string operatorName = "";

        //Datetime
        public string hour1 { get; set; }
        TimeSpan? startHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;
        public class ItemModel
        {
            public int EffectId { get; set; }
            public string Benefit { get; set; }
        }

        List<ItemModel> items = new List<ItemModel>();
        bool showLoading = true;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["kaizen"], href: "/kaizen"),
                    new BreadcrumbItem(text: Localizer["create kaizen"], href: "", disabled: true)
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

            _kaizen.CreateDate = DateTime.Now;

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();

            if (user.UserType == 2)
            {
                plantId = (int)user.PlantId;
                areaId = 0;

                _areas = user.Areas.ToList();

                ssvId = user.UserId;
                foreach (var sv in user.Subordinates.ToList())
                {
                    _supervisors.Add(sv);
                }

                
            }
            else if(user.UserType == 3) 
            {
                plantId = (int)user.PlantId;
                areaId = 0;
                if(user.Areas != null &&  user.Areas.Count > 0)
                    areaId = user.Areas.FirstOrDefault().AreaId;
                _areas = await AreaServices.GetAreas((int)user.PlantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();

                ssvId = (int)user.SuperiorId;
                supervisorId = user.UserId;

                ssvName = user.Superior.Name;
                svName = user.Name;

                _seniorSupervisors.Add(user.Superior);
                _supervisors.Add(user);
                foreach(var op in user.Subordinates.ToList())
                {
                    operatorUsers.Add(op);
                }
            }
            showLoading = false;
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
                svName = _supervisors.Where(sv => sv.UserId == supervisorId).FirstOrDefault().Name;

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
            if(ssvId != 0)
                ssvName = _seniorSupervisors.Where(ssv => ssv.UserId == ssvId).FirstOrDefault().Name;

            supervisorId = 0;
            operatorId = 0;
            _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 3, false, false);
            _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            StateHasChanged();
        }
        private async void OperatorName()
        {
            if (operatorId != 0)
                operatorName = operatorUsers.Where(op => op.UserId == operatorId).FirstOrDefault().Name;

        }


        private async Task CreateNewKaizen()
        {

            _kaizen.PlantId = plantId;
            _kaizen.AreaId = areaId;

            _kaizen.SeniorSupervisorId = ssvId;
            _kaizen.SupervisorId = supervisorId;
            _kaizen.ProposedId = operatorId;
            _kaizen.Status = 1;


            FormatDate();

            foreach (var item in items)
            {
                var kaizenTransaction = new KaizenTransaction
                {
                    Title = item.EffectId.ToString(), 
                    Description = item.Benefit,
                    Type = 1,
                    IsActive = true
                };

                _kaizen.Transactions.Add(kaizenTransaction);
            }

            var result = await KaizenServices.CreateKaizen(_kaizen);

            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Kaizen Created", Severity.Info);

                _kaizen = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/kaizen");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

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
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new List<string>();
        private List<string> capturedImagesThen = new List<string>();

        private bool visibleCamera = false;
        private int imageIndex = 0;

        private void OpenCameraDialog(int index)
        {
            imageIndex = index;
            visibleCamera = true;
        }

        private CameraStreamer CameraStreamerReference = new();

        private string? cameraId = null;

        private int frameCount;

        private string imageData;

        private bool _cameraInitialized = false;
        private bool _cameraAccessGranted = false;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_cameraInitialized)
            {
                try
                {

                    frameCount = 0;
                    _cameraAccessGranted = await CameraStreamerReference.GetCameraAccessAsync();

                    if (_cameraAccessGranted)
                    {
                        await CameraStreamerReference.ReloadAsync();
                        _cameraInitialized = true;
                    }
                    else
                    {
                        Console.WriteLine("Permiso de acceso a la cámara no otorgado.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al recargar la cámara: {ex.Message}");
                }
            }


        }


        private async void Stop()
        {
            frameCount = 0;
            await CameraStreamerReference.StopAsync();
        }

        private void OnFrameHandler(string _)
        {
            try
            {

                ++frameCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recargar la cámara: {ex.Message}");
            }
        }

        private async void GetCurrentFrame()
        {
            try
            {

                imageData = await CameraStreamerReference.GetCurrentFrameAsync();

                if (!string.IsNullOrEmpty(imageData))
                {
                    if (imageIndex == 1)
                    {
                        capturedImages.Add(imageData);
                    }
                    else
                    {
                        capturedImagesThen.Add(imageData);
                    }
                }
                visibleCamera = false;
                Stop();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recargar la cámara: {ex.Message}");
            }
        }

        private void RemoveImage(int index, int imgIndex)
        {

            if (index >= 0 && index < capturedImages.Count || index < capturedImagesThen.Count)
            {
                if (imgIndex == 1)
                {
                    capturedImages.RemoveAt(index);
                }
                else
                {
                    capturedImagesThen.RemoveAt(index);
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
            await UploadImages(capturedImages, _kaizen.KaizenId, true);
            await UploadImages(capturedImagesThen, _kaizen.KaizenId, false);

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
                        else
                        {
                            capturedImagesThen.Add(mediaUri);
                        }
                    }
                }
            }
        }

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

        private bool visibleSign = false;
        private int userTypeSign = 0;
        private void OpenSignComment(int userType)
        {
            visibleSign = true;
            userTypeSign = userType;
        }
        void CloseSign() => visibleSign = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

        public async Task SignDate()
        {
            if(userTypeSign == 1)
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
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

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