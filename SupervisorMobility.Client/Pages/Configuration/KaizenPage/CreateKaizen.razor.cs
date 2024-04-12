using BlazorCameraStreamer;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.Configuration.KaizenPage
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

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                    new BreadcrumbItem(text: Localizer["kaizen"], href: "", disabled: true)
                };

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
                if (operatorUser.AreaId == areaId && operatorUser.SuperiorId == supervisorId)
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

            var result = await KaizenServices.CreateKaizen(_kaizen);

            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Kaizen Created", Severity.Info);

                _kaizen = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/configuration");
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
                if(imageIndex == 1) { 
                    capturedImages.Add(imageData);
                }
                else
                {
                    capturedImagesThen.Add(imageData);
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

                    var base64Data = imageData.Replace("data:image/png;base64,", "");

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
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("No images to upload", Severity.Warning);
            }
        }

        private async Task<AsyncVoidMethodBuilder> UploadEvidence()
        {
            await UploadImages(capturedImages, _kaizen.KaizenId, true);
            await UploadImages(capturedImagesThen, _kaizen.KaizenId, false);

            return new AsyncVoidMethodBuilder();
        }

    }
}