using Blazorise.Extensions;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using SupervisorMobility.Client.Services.AssyChartService;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class PlanJobObservation
    {

        [Parameter]
        public string date { get; set; }
        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? startHour = new TimeSpan(00, 00, 00);
        TimeSpan? endHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;
        DateTime newDate2;

        List<JobObservation> _jobObservations;
        List<Plant> _plants { get; set; } = new();
        List<Product> _products { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();

        List<User> _supervisors { get; set; } = new();
        List<User> _allSupervisors = new();

        List<AssyChart> _assycharts { get; set; }

        List<Lup> _tempLup { get; set; } = new();
        Lup lup { get; set; } = new();
        List<Lup> _lup { get; set; } = new();

        public JobObservation _jobObservation { get; set; } = new();

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cycles = new string[5] { "", "", "", "", "" };

        //timer
        const string DEFAULT_TIME = "00:00:00.000";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        //Past Job observation
        //Lup Modal
        private bool visiblePast = false;
        private bool visibleLup = false;
        private int lupId;

        private DialogOptions dialogLup = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Past job observation
        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();

        public Distribution distribution = new Distribution();
        public Operation operation = new();

        public bool flag = false;


        // Breadcrumb links
        private List<BreadcrumbItem> _links;


        //User
        private string json = string.Empty;
        public User user = new();

        //Operator user
        public List<User> _operators = new();
        public List<User> operatorUsers = new();




        protected async override Task OnInitializedAsync()
        {
             _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation"),
                new BreadcrumbItem(text: Localizer["planJobObservation"], href: "", disabled: true)
            };

            _jobObservation.Supervisor = new();

            await GetUserAsync();



            if (user != null)
            {

                _plants = await PlantServices.GetPlants();

                if (user.UserType == 1)
                {
                    _jobObservation.PlantId = 0;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    _allSupervisors = await UsersService.GetUsersByType(3, true, false);
                    _operators = await UsersService.GetUsersByType(4, true, false);

                }
                else if (user.UserType == 2)
                {
                    _jobObservation.PlantId = (int)user.PlantId;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    _allSupervisors = await UsersService.GetUsersByType(3, true, false);
                    _operators = await UsersService.GetUsersByType(4, true, false);

                }
                else if(user.UserType == 3)
                {

                    _jobObservation.PlantId = (int)user.PlantId;

                    _jobObservation.AreaId = (int)user.AreaId;

                    _areas = await AreaServices.GetAreas((int)user.PlantId);
                    _jobObservation.SupervisorId = user.UserId;
                    _jobObservation.Supervisor = await UsersService.GetUser(user.UserId);

                    _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);


                    //operator User
                    _operators = await UsersService.GetUsersByType(4, true, false);
                    foreach (var operatorUser in _operators)
                    {
                        if (user != null && operatorUser.AreaId == user.AreaId && operatorUser.SuperiorId == user.UserId)
                        {
                            operatorUsers.Add(operatorUser);
                        }
                    }

                }

                StateHasChanged();
            }



            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            date = date.Replace("-", "/");



            _jobObservation.IsActive = true;
            _jobObservation.StartDate = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
            _jobObservation.EndDate = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
            _jobObservation.Option = 1;


            _plants = await PlantServices.GetPlants();
            //_products = await ProductService.GetProducts();


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
            _jobObservation.AreaId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _jobObservation.OperatorId = 0;
            _jobObservation.SupervisorId = 0;
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _jobObservation.SupervisorId = 0;
            _supervisors.Clear();
            if(user.UserType == 1)
            {
                foreach (User sv in _allSupervisors)
                {
                    if (sv.PlantId == _jobObservation.PlantId && sv.AreaId == _jobObservation.AreaId)
                    {
                        _supervisors.Add(sv);
                    }
                }

            }
            else if(user.UserType == 2) 
            {
                foreach (User sv in _allSupervisors)
                {
                    if (sv.PlantId == _jobObservation.PlantId && sv.AreaId == _jobObservation.AreaId && sv.SuperiorId == user.UserId)
                    {
                        _supervisors.Add(sv);
                    }
                }
            }

            _jobObservation.OperatorId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
            StateHasChanged();
        }

        private void ShowOperators()
        {
            if(_jobObservation.DistributionId != 0 && _jobObservation.OperationId != 0)
                ShowPastJobObservations();
            operatorUsers = new();
            _jobObservation.OperatorId = 0;
            //operator User
            foreach (var operatorUser in _operators)
            {
                if (operatorUser.AreaId == _jobObservation.AreaId && operatorUser.SuperiorId == _jobObservation.SupervisorId)
                {
                    operatorUsers.Add(operatorUser);
                }
            }
            StateHasChanged();
        }

        private async void ShowOperations()
        {

            _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
            _jobObservation.OperationId = 0;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
            _assycharts = await AssychartsServices.GetAssyChartsByDistribution(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            await Task.Delay(150);
            distribution = await DistributionService.GetDistributionById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            StateHasChanged();
        }

        private async void ShowPastJobObservations()
        {
            flag = true;
            operation = await OperationService.GetOperationById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, _jobObservation.OperationId);
            pastjobObservations = new();
            pastLup = new();
            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                        && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
                    {

                        pastjobObservations.Add(job);

                        pastJob = await JobObservationService.GetJobObservationWithLup(job.JobObservationId);
                        foreach (var lups in pastJob.Lup)
                        {
                            pastLup.Add(lups);
                        }
                    }

                }


            }
            pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();
            StateHasChanged();
        }

        private async Task PlanNewJobObservation()
        {
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {



                if(user.UserType == 1)
                {
                    if(_jobObservation.SupervisorId == 0)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Select a Supervisor first", Severity.Error);
                        return;
                    }
                    _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);
                }

                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                //Planned
                _jobObservation.Type = 1;

                Console.WriteLine(startHour);
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour1);


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour2);

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                _jobObservation.Status = 1;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
                        {
                            temp.JobObservationId = _jobObservations.Last().JobObservationId;
                            var result2 = await LupService.CreateLup(temp);
                            if (result2 != null)
                            {
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add($"Job observation Lup item Created", Severity.Info);
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add($"Error in Lup", Severity.Error);
                            }
                        }
                    }

                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
            }
            else
            {
                Console.WriteLine(_jobObservation.StartDate);

                //Planned
                _jobObservation.Type = 1;

                Console.WriteLine(startHour);
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour1);


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour2);

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                _jobObservation.Status = 1;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
                        {
                            temp.JobObservationId = _jobObservations.Last().JobObservationId;
                            var result2 = await LupService.CreateLup(temp);
                            if (result2 != null)
                            {
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add($"Job observation Lup item Created", Severity.Info);
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add($"Error in Lup", Severity.Error);
                            }
                        }
                    }

                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
            }



        }

        void CancelCreateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }

        private void OpenDialogLup(int id)
        {
            lupId = id;
            visibleLup = true;
        }

        private void OpenDialogPastJobObservations()
        {

            if (!flag)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["selectADistributionAndAnOperation"], Severity.Warning);
                return;
            }
            if(_jobObservation.SupervisorId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstSelectASupervisor"], Severity.Warning);
                return;
            }
            visiblePast = true;
        }

        void CloseLup() => visibleLup = false;
        void CloseOverdue() => visiblePast = false;

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        //Lup
        void Closed(MudChip chip)
        {
            // react to chip closed
        }


        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool searchAssychart = false;

        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderError = false;
        private string messageErrorFolders;


        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        private async void OpenDialogGOS(string ruta)
        {
            GOSrute = ruta;
            GosDialog = true;
            folderError = false;

            Console.WriteLine($"gos {ruta}");

            GosFilesInFolder = new CDMS_GOS_Archives();

            GosFilesInFolder = await CDMSServices.GetFilesGOS(ruta);
            if (GosFilesInFolder == null)
                folderError = true;


            StateHasChanged();
        }
        void CloseGos() => GosDialog = false;

        private async void OpenDialogCcp(string ruta)
        {
            CCPrute = ruta;
            CcpDialog = true;
            folderError = false;
            Console.WriteLine($"Cpc {ruta}");

            CcpFilesInFolder = new CDMS_CCP_Archives();
            CcpFilesInFolder = await CDMSServices.GetFilesCCP(ruta);
            if (CcpFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseCcp() => CcpDialog = false;

        private async void OpenDialogHoe(string ruta)
        {
            HOErute = ruta;
            HoeDialog = true;
            Console.WriteLine($"hoe {ruta}");

            folderError = false;
            HoeFilesInFolder = new CDMS_HOE_Archives();
            HoeFilesInFolder = await CDMSServices.GetFilesHOE(ruta);
            if (HoeFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseHoe() => HoeDialog = false;


        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

        private async Task DownloadFileFromURL_HOE(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }
        private async Task DownloadFileFromURL_CCP(string urlroute, string namefile)
        {

            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkCCP(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempCCP(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }
        }
        private async Task DownloadFileFromURL_GOS(string urlroute, string namefile)
        {
            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkGOS(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempGOS(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }

        }
    }
}
