using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }

        public JobObservation _jobObservation { get; set; } = new();
        public Lup lup { get; set; } = new();
        public JobObservation _lupJobObservations { get; set; } = new();

        public string hour1 { get; set; }
        public string hour2 { get; set; }

        DateTime newDate1;
        DateTime newDate2;

        //Objects
        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Product> _products { get; set; } = new();
        private AssyChart _assychart { get; set; } = new AssyChart();
        private string messageErrorFolders;
        private bool searchAssychart = false;


        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cicles = new string[5];

        //timer
        const string DEFAULT_TIME = "00:00:00.000";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;
        public int opt = 1;

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;


        //Lup Modal
        private bool visibleLup = false;
        private bool visiblePast = false;
        private int lupId;

        private DialogOptions dialogLup = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Past job observation
        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("Update Job Observation", href: "", disabled: true)
        };

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();
        void Closed(MudChip chip)
        {
            // react to chip closed
        }
        protected async override Task OnInitializedAsync()
        {
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
                _jobObservation.Supervisor = new();
                //glosary
                glosary = await GlosaryService.GetGlosary();
                _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

                _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);

                _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);


                startHour = _jobObservation.DateStart?.TimeOfDay;
                endHour = _jobObservation.DateEnd?.TimeOfDay;

                _plants = await PlantServices.GetPlants();
                //_products = await ProductService.GetProducts();
                _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

                _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;

                var prod = _jobObservation.Models.Split('|');
                models[0] = Int32.Parse(prod[0]);
                models[1] = Int32.Parse(prod[1]);
                models[2] = Int32.Parse(prod[2]);
                models[3] = Int32.Parse(prod[3]);
                models[4] = Int32.Parse(prod[4]);
                cicles = _jobObservation.Cicles.Split('|');

                users = await UsersService.GetUsers();
                foreach (var operatorUser in users)
                {
                    if (user != null && operatorUser.AreaId == operatorUser.AreaId && operatorUser.IsOperator)
                    {
                        operatorUsers.Add(operatorUser);
                    }
                }


                await GetUserAsync();

                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.DateStart?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.DateStart?.ToShortDateString()).Date
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
                pastjobObservations = pastjobObservations.OrderBy(x => x.DateStart).ToList();

                if (_jobObservation.PlantId != 0)
                {
                    if (_jobObservation.AreaId != 0)
                    {
                        if (_jobObservation.DistributionId != 0)
                        {
                            if (_jobObservation.DistributionId != 0)
                            {

                                _assychart = await AssychartServices.GetAssyChartAdvance(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, _jobObservation.OperationId);
                                if (_assychart == null)
                                    messageErrorFolders = "The folders with the information provided were not located.";
                                else
                                    searchAssychart = true;

                            }
                            else
                            {
                                messageErrorFolders = "Job Observation does not contain a valid operation";
                                Console.WriteLine("missing plant");
                            }
                        }
                        else
                        {
                            messageErrorFolders = "Job Observation does not contain a valid distribution";
                            Console.WriteLine("missing plant");
                        }
                    }
                    else
                    {
                        messageErrorFolders = "Job Observation does not contain a valid area";
                        Console.WriteLine("missing plant");
                    }
                }
                else
                {
                    messageErrorFolders = "Job Observation does not contain a valid plant";
                    Console.WriteLine("missing plant");
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


        //Job observations
        private async void ShowAreas()
        {
            _jobObservation.AreaId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private async void ShowOperations()
        {
            _jobObservation.OperationId = 0;
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }
        private async Task EditJobObservation()
        {

            hour1 = _jobObservation.DateStart?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.DateEnd?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date Start", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour1);
            }


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date End", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour2);
            }
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles = cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];
            _jobObservation.DateStart = newDate1;
            _jobObservation.DateEnd = newDate2;
            _jobObservation.Status = 2;

            if (_jobObservation.Justification == "")
            {
                _jobObservation.Justification = null;
            }

            var result = await JobObservationService.UpdateJobObservation(_jobObservation);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Updated", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        void CancelUpdateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }




        //timer
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            TimeSpan hundreths;
            int centiseconds = 0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = (int)hundreths.TotalMilliseconds / 10;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cicles[0] = centiseconds.ToString(); break;
                case 2:
                    cicles[1] = centiseconds.ToString(); break;
                case 3:
                    cicles[2] = centiseconds.ToString(); break;
                case 4:
                    cicles[3] = centiseconds.ToString(); break;
                case 5:
                    cicles[4] = centiseconds.ToString(); break;
            }
            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0, 12);
            StateHasChanged();
        }

        void StartTimer()
        {
            startTime = DateTime.Now;
            timer = new System.Timers.Timer(1);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            isRunning = true;
        }

        void StopTimer()
        {
            TimeSpan hundreths;
            int centiseconds = 0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = (int)hundreths.TotalMilliseconds / 10;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cicles[0] = centiseconds.ToString(); break;
                case 2:
                    cicles[1] = centiseconds.ToString(); break;
                case 3:
                    cicles[2] = centiseconds.ToString(); break;
                case 4:
                    cicles[3] = centiseconds.ToString(); break;
                case 5:
                    cicles[4] = centiseconds.ToString(); break;
            }
            isRunning = false;
            Console.WriteLine($"Elapsed Time: {elapsedTime}");
            timer.Enabled = false;
            elapsedTime = DEFAULT_TIME;

        }

        void OnTimerChanged()
        {
            if (!isRunning)
                StartTimer();
            else
                StopTimer();
        }

        public async void FinalizeJobObservation()
        {

            if(_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Operator Signature", Severity.Error);
                return;
            }

            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles = cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];
            _jobObservation.DateFinalized = DateTime.Now;
            Console.WriteLine(_jobObservation.DateFinalized);
            _jobObservation.Status = 4;

            var result = await JobObservationService.UpdateJobObservation(_jobObservation);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Finalized", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert

        }





        //Lup

        public async void AddLup(int pillar)
        {

            switch (pillar)
            {
                case 1: 
                    if(areaS != null && areaS.Length > 0)
                    {
                        lup.Oportunity = areaS;
                        areaS = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error S Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 2:
                    if (areaQ != null && areaQ.Length > 0)
                    {
                        lup.Oportunity = areaQ;
                        areaQ = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error Q Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 3:
                    if (areaD != null && areaD.Length > 0)
                    {
                        lup.Oportunity = areaD;
                        areaD = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error D Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 4:
                    if (areaC != null && areaC.Length > 0)
                    {
                        lup.Oportunity = areaC;
                        areaC = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error C Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 5:
                    if (areaOther != null && areaOther.Length > 0)
                    {
                        lup.Oportunity = areaOther;
                        areaOther = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error Others Area is empty", Severity.Error);
                        return;
                    }
                    break;

            }

            lup.Observer = _jobObservation.Supervisor.Name;
            lup.JobObservationId = _jobObservation.JobObservationId;
            lup.Pillar = pillar;
            lup.Status = 1;
            lup.CreatedDate= DateTime.Now;
            lup.IsActive = true;


            var result = await LupService.CreateLup(lup);
            if (result != null)
            {
                _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);

                await GetUserAsync();

                pastjobObservations = new();
                pastLup = new();
                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.DateStart?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.DateStart?.ToShortDateString()).Date
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
                pastjobObservations = pastjobObservations.OrderBy(x => x.DateStart).ToList();
                StateHasChanged();

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup Created", Severity.Info);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Lup", Severity.Error);
            }
        }

        //lup modal
        private void OpenDialogLup(int id)
        {
            lupId = id;
            visibleLup = true;
        }

        private void OpenDialogPastJobObservations()
        {
            visiblePast = true;
        }

        void CloseLup() => visibleLup = false;
        void CloseOverdue() => visiblePast = false;
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        async Task DeleteLup(int lupId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this lup?");

            if (confirm)
            {
                await LupService.DeleteLup(lupId);

                _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);

                await GetUserAsync();

                pastjobObservations = new();
                pastLup = new();
                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.DateStart?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.DateStart?.ToShortDateString()).Date
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
                pastjobObservations = pastjobObservations.OrderBy(x => x.DateStart).ToList();

                StateHasChanged();

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup Deleted", Severity.Info);


            }
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }


        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool folderError = false;


        private CDMS_CCP_Document CcpFilesInFolder = new CDMS_CCP_Document();
        private CDMS_HOE_Document HoeFilesInFolder = new CDMS_HOE_Document();
        private CDMS_GOS_Document GosFilesInFolder = new CDMS_GOS_Document();

        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        private async void OpenDialogGOS(string ruta)
        {
            GOSrute = ruta;
            GosDialog = true;
            folderError = false;

            Console.WriteLine($"gos {ruta}");

            GosFilesInFolder = new CDMS_GOS_Document();

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

            CcpFilesInFolder = new CDMS_CCP_Document();
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
            HoeFilesInFolder = new CDMS_HOE_Document();
            HoeFilesInFolder = await CDMSServices.GetFilesHOE(ruta);
            if (HoeFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseHoe() => HoeDialog = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

    }
}
