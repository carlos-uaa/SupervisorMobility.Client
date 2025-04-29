using Blazorise.Extensions;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using SupervisorMobility.Client.Services.AssyChartService;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Timers;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
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

        AssyChart _assychart { get; set; }

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
        string[] HoeTimes = new string[5] { "", "", "", "", "" };

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
        public Operation? operation = new();

        public bool flag = false;


        // Breadcrumb links
        private List<BreadcrumbItem> _links;


        //User
        private string json = string.Empty;
        public User user = new();

        //Operator user
        public List<User> _operators = new();
        public List<User> operatorUsers = new();

        public int kpiID = 0;


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

            date = date.Replace("-", "/");

            _jobObservation.IsActive = true;
            _jobObservation.StartDate = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
            _jobObservation.EndDate = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
            _jobObservation.Option = 1;

            StateHasChanged();
            if (user != null)
            {


                _plants = await PlantServices.GetPlants();
                _plants = _plants.OrderBy(p => p.Description).ToList();

                if (user.UserType == 1)
                {
                    _jobObservation.PlantId = 0;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    _allSupervisors = await UsersService.GetUsersByType(3, true, false);
                    _allSupervisors = _allSupervisors.OrderBy(s => s.Name).ToList();

                    _operators = await UsersService.GetUsersByType(4, true, false);
                    _operators = _operators.OrderBy(o => o.Name).ToList();
                }
                else if (user.UserType == 2)
                {
                    _jobObservation.PlantId = (int)user.PlantId;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    _allSupervisors = await UsersService.GetUsersByType(3, true, false);
                    _allSupervisors = _allSupervisors.OrderBy(s => s.Name).ToList();

                    _operators = await UsersService.GetUsersByType(4, true, false);
                    _operators = _operators.OrderBy(o => o.Name).ToList();

                }
                else if(user.UserType == 3)
                {

                    _jobObservation.PlantId = (int)user.PlantId;

                    _jobObservation.AreaId = (int)user.AreaId;

                    _areas = await AreaServices.GetAreas((int)user.PlantId);
                    _areas = _areas.OrderBy(a => a.Description).ToList();

                    _jobObservation.SupervisorId = user.UserId;
                    _jobObservation.Supervisor = await UsersService.GetUser(user.UserId);

                    _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                    _distributions = _distributions.OrderBy(d => d.Description).ToList();   

                    //operator User
                    _operators = await UsersService.GetUsersByType(4, true, false);
                    _operators = _operators.OrderBy(o => o.Name).ToList();

                    foreach (var operatorUser in _operators)
                    {
                        if (user != null && operatorUser.AreaId == user.AreaId && operatorUser.SuperiorId == user.UserId)
                        {
                            operatorUsers.Add(operatorUser);
                        }
                    }

                }

                glosary = await GlosaryService.GetGlosary();
                _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);


                StateHasChanged();

                try
                {
                    CCPFolders = await CDMSServices.GetFoldersCCP();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Get CCP Folder From CCP");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.Message);
                }

                if (CCPFolders != null)
                {
                    //folderCCPError = false;
                    rootNodeCCP = TreeServices.Make_Tree_CCP(CCPFolders.operation);
                }
                else
                {
                    //folderCCPError = true;
                }

            }

            var _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories();

            _jobObservation.SectionIds = string.Join("|", _checklistCategoriesAndQuestions
                .Select(c => c.JobCategoryStructureId));


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
            _jobObservation.Operations = new List<Operation>();
            _jobObservation.OperatorId = 0;
            _jobObservation.SupervisorId = 0;
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();
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
            _jobObservation.Operations = new List<Operation>();
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
            _distributions = _distributions.OrderBy(d => d.Description).ToList();
            StateHasChanged();
        }

        private void ShowOperators()
        {
            if(_jobObservation.DistributionId != 0 && _jobObservation.Operations?.FirstOrDefault()?.OperationId != 0)
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
            _products = _products.OrderBy(p => p.Description).ToList();

            _jobObservation.Operations = new List<Operation>();
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
            _operations = _operations.OrderBy(o => o.Description).ToList();

            _assychart = await AssychartsServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            await Task.Delay(150);
            distribution = await DistributionService.GetDistributionById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            StateHasChanged();
        }

        private async void ShowPastJobObservations()
        {
            flag = true;

            if(_jobObservation.Operations.Count() > 0)
                operation = await OperationService.GetOperationById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId,(int) _jobObservation.Operations?.FirstOrDefault().OperationId);
            
            pastjobObservations = new();
            pastLup = new();
            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                        && job.DistributionId == _jobObservation.DistributionId && job.Operations?.FirstOrDefault()?.OperationId == _jobObservation.Operations?.FirstOrDefault()?.OperationId)
                    {

                        pastjobObservations.Add(job);

                        pastJob = await JobObservationService.GetJobObservationById(job.JobObservationId, true, true, true, false, false);
                        foreach (var lups in pastJob.Lup)
                        {
                            pastLup.Add(lups);
                        }
                    }

                }


            }
            pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();

            if (_assychart != null && _assychart.RoutesProductsAssyChart?.Count > 0)
            {
                listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(operation.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                FilterOperation = true;
            }

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


            if (user.UserType == 1)
            {
                if (_jobObservation.SupervisorId == 0)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Select a Supervisor first", Severity.Error);
                    return;
                }
                _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);
            }

            _jobObservation.KpiId = kpiID;
            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            //_jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            //_jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Status = 1;

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {


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



                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;


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


                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

            }

            var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Planned", Severity.Info);

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

      

        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

   
     

        public int idFilter;

        MudTabPanel HOE;
        MudTabPanel HOECD;
        MudTabPanel CCP;
        MudTabPanel CCPCD;
        MudTabPanel GOS;
        MudTabPanel GOSCD;

        public bool CodePathModalDisplay { get; set; } = false;
        private string searchCodeString = "";
        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        SOSCodePath CodePathDialogDisplay { get; set; }

        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }
        int SOSCodePathId { get; set; } = 0;
        string SosPanelOpen { get; set; } = "";
        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, MudTabPanel panelSelect)
        {
            ShowLoading = true;
            SOSCodePathId = itemselected.SOSCodePathId;

            if (panelSelect == HOE)
            {
                SosPanelOpen = "HOE";
            }
            else if (panelSelect == CCP)
            {
                SosPanelOpen = "CCP";
            }
            else if (panelSelect == GOS)
            {
                SosPanelOpen = "GOS";
            }
            else if (panelSelect == HOECD)
            {
                SosPanelOpen = "HOE_CD";
            }
            else if (panelSelect == CCPCD)
            {
                SosPanelOpen = "CCP_CD";
            }
            else if (panelSelect == GOSCD)
            {
                SosPanelOpen = "GOS_CD";
            }

            CodePathModalDisplay = true;
            StateHasChanged();

            return new AsyncVoidMethodBuilder();
        }
 

        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();
        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();


        bool ShowOperationsDialog { get; set; } = false;


        private async void OpenOperations()
        {
            ShowOperationsDialog = true;

            StateHasChanged();

        }

        private async void CloseOperations()
        {
            ShowOperationsDialog = false;

            StateHasChanged();

        }

        private string searchTerm = "";
        private IEnumerable<Operation> FilteredOperations =>
            _operations.Where(op =>
                string.IsNullOrEmpty(searchTerm) ||
                (op.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));

        private async Task DisableNeedOfSSV()
        {
            _jobObservation.WillNotRequireSSVApproval = true;
            Snackbar.Add(Localizer["SSVReqFalse"], Severity.Warning);
            StateHasChanged();
        }
        private async Task EnableNeedOfSSV()
        {
            _jobObservation.WillNotRequireSSVApproval = false;
            Snackbar.Add(Localizer["SSVReqTrue"], Severity.Success);
        }
        private async Task UpdateOperator()
        {
            var operatorUser = operatorUsers.FirstOrDefault(p => p.UserId == _jobObservation.OperatorId);
            if (operatorUser != null)
            {
                _jobObservation.Operator = operatorUser;
            }
        }
    }
}
