using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.PaginationEntities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Pages.Inicio.JobObservationPage.Modals;
using SupervisorMobility.Client.Services.BreadcrumsService;
using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class JobObservationPage
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }
        //Objects
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";

        public Color color = Color.Default;
        public bool filters = false;
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();

        public bool operationFlag = false;
        public int distributionId;
        public int operationId;
        public DateTime? filterDate = null;
        public int operatorId;
        public int statusId;
        public int idFilter;
        public int typeId;

        //Filters
        public List<JobObservation> _filterJobObservation { get; set; } = new();



        //Job observations status lists.
        public List<JobObservation> _jobObservations { get; set; } = new();

        JOCountPaginationDto JOCounting { get; set; } = new JOCountPaginationDto
        {
            DistributionCount = new(),
            OperationCount = new(),
            OperatorCount = new(),
            StatusCount = new List<JOCount> { new(), new(), new(), new(), new(), new(), new(), new() }
        };
        bool anyItems { get; set; }
        int TotalItems { get; set; }


        //Admin
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();

        public string totalJobObservations;
        public string totalPlanned;
        public string totalInProgress;
        public string totalLate;
        public string totalUnderReview;
        public string totalRejected;
        public string totalFinished;
        public string totalProgrammed;
        public string totalTraining;


        public int plantId;
        public int areaId;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();


        bool showLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

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


            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation", disabled: true),
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
                return;
            }

            await GetUserAsync();
            //_jobObservations.Clear();
            //ClearFilters();

            if (user != null)
            {
                //var jobObservationsTask = user.UserType != 3
                //    ? JobObservationService.GetAllJobObservations(true, true)
                //    : JobObservationService.GetAllJobObservations(true, true, idUser: user.UserId);
                Task<List<Plant>> plantsTask = null;
                Task<List<Area>> areasTask = null;
                if (user.UserType == 3)
                {
                    plantId = (int)user.PlantId;
                    areaId = (int)user.AreaId;
                    _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);
                    operatorUsers = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, true, false);
                }
                else
                {
                    if (user.UserType == 1 || user.UserType == 5)
                    {
                        plantsTask = PlantServices.GetPlants();
                    }

                    if (user.UserType == 2 || user.UserType == 5)
                    {
                        plantId = (int)user.PlantId;
                        areasTask = AreaServices.GetAreas(plantId);
                    }
                }

                //_jobObservationsAux = await jobObservationsTask;

                if (plantsTask != null)
                {
                    _plants = (await plantsTask).OrderBy(p => p.Description).ToList();
                }

                if (areasTask != null)
                {
                    _areas = (await areasTask).OrderBy(a => a.Description).ToList();
                }

                //if (user.UserType == 1 || user.UserType == 6)
                //{
                //    _jobObservations = _jobObservationsAux.ToList();
                //}
                //else if (user.UserType == 2)
                //{
                //    plantId = (int)user.PlantId;
                //    _jobObservations = _jobObservationsAux
                //        .Where(jobobs => jobobs.PlantId == plantId &&
                //                         user.Subordinates.Any(usr => usr.UserId == jobobs.SupervisorId))
                //        .ToList();
                //}
                //else if (user.UserType == 3)
                //{
                //    plantId = (int)user.PlantId;
                //    areaId = (int)user.AreaId;
                //    _jobObservations = _jobObservationsAux.ToList();
                //}
                //else if (user.UserType == 5)
                //{
                //    plantId = (int)user.PlantId;
                //    _jobObservations = _jobObservationsAux
                //        .Where(jobobs => jobobs.PlantId == plantId &&
                //                         user.Subordinates.Any(usr => usr.UserId == jobobs.Supervisor.SuperiorId))
                //        .ToList();
                //}

                //_filterJobObservation = _jobObservations;
                //JobObservationsTotalCount();
                StateHasChanged();
            }


            showLoading = false;
        }

        async Task<TableData<JobObservation>> LoadJobObs(TableState state)
        {
            (int Total, List<JobObservation> JobObservations, JOCountPaginationDto Count) response = new();

            DateTime? dateToFilter = filterDate ?? default;

            response = await JobObservationService.GetAllJobObservationsByFilters(dateToFilter.Value, dateToFilter.Value, idFilter, plantId, areaId,
                distributionId, operationId, operatorId, statusId, user.UserId, typeId, searchString, state.Page + 1, state.PageSize, (int)state.SortDirection, state.SortLabel);

            var pps = response.JobObservations.ToArray();

            _jobObservations = pps.ToList();

            JOCounting = response.Count;

            if (typeId == default && statusId == default)
            {
                TotalItems = response.Total;
            }
            else
            {
                TotalItems = response.Count.StatusCount.Sum(item => item.count);
            }

            anyItems = response.Total == 0;

            JobObservationsTotalCount();

            StateHasChanged();

            return new TableData<JobObservation>() { Items = pps, TotalItems = response.Total };
        }

        private void JobObservationsTotalCount()
        {
            totalJobObservations = Localizer["allJobObservations"] + " (" + TotalItems + ")";
            totalPlanned = Localizer["planned"] + " (" + JOCounting.StatusCount[0].count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + JOCounting.StatusCount[1].count + ")";
            totalLate = Localizer["late"] + " (" + JOCounting.StatusCount[2].count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + JOCounting.StatusCount[3].count + ")";
            totalRejected = Localizer["rejected"] + " (" + JOCounting.StatusCount[4].count + ")";
            totalFinished = Localizer["finished"] + " (" + JOCounting.StatusCount[5].count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + JOCounting.StatusCount[6].count + ")";
            totalTraining = Localizer["VerifyTraining"] + " (" + JOCounting.StatusCount[7].count + ")";

        }

        private void OnDateChange(DateTime? date)
        {
            filterDate = date;
            Filters();
        }


        private async void ShowAreas()
        {

            if (plantId == 0)
            {
                areaId = 0;
                ClearFilters();
                await SelectTableEvent0.ReloadServerData();
                StateHasChanged();
                return;

            }

            areaId = 0;
            color = Color.Default;
            filters = false;
            ClearFilters();
            operatorUsers.Clear();


            await SelectTableEvent0.ReloadServerData();


            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();

            StateHasChanged();
        }


        private async void ShowJobObs()
        {
            if (areaId == 0)
            {
                ShowAreas();
                return;
            }

            ClearFilters();
            operatorUsers.Clear();

            await SelectTableEvent0.ReloadServerData();

            _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);

            operatorUsers = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, true, false);

            StateHasChanged();
        }

        public void ActiveFilters()
        {
            if (!filters && areaId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Select an Area first", Severity.Warning);
                return;
            }
            filters = !filters;

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();

            SelectTableEvent0.ReloadServerData();

            if (color == Color.Info)
            {
                color = Color.Default;
            }
            else
            {
                color = Color.Info;
            }

        }


        public void ClearFilters()
        {

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();

            SelectTableEvent0.ReloadServerData();
            StateHasChanged();
        }

        //Filters
        private void FilterDistributions()
        {

            if (distributionId == 0)
            {
                SelectTableEvent0.ReloadServerData();
                return;
            }

            operationId = new();


            operationFlag = true;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == distributionId)].Operations;


            SelectTableEvent0.ReloadServerData();
        }

        private void Filters()
        {
            SelectTableEvent0.ReloadServerData();
            //_jobObservations = _filterJobObservation;


            //if (distributionId != default(int))
            //{
            //    _jobObservations = _jobObservations.Where(jobObs => jobObs.DistributionId == distributionId).ToList();

            //}
            //if (operationId != default(int))
            //{
            //    _jobObservations = _jobObservations.Where(jobObs => jobObs.Operations?.FirstOrDefault()?.OperationId == operationId).ToList();

            //}
            //if (statusId != default(int))
            //{
            //    _jobObservations = _jobObservations.Where(jobObs => jobObs.Status == statusId).ToList();

            //}
            //if (operatorId != default(int))
            //{
            //    _jobObservations = _jobObservations.Where(jobObs => jobObs.OperatorId == operatorId).ToList();

            //}
            //if (filterDate != null)
            //{
            //    _jobObservations = _jobObservations.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();

            //}
            //if (idFilter != default(int))
            //{
            //    _jobObservations = _jobObservations.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();

            //}

            JobObservationsTotalCount();
        }

        public void ClearStatus(int sts, int filterType = default)
        {
            statusId = sts;
            typeId = filterType == 4 ? 4 : default;
            selectedRowNumber = -1;
        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
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


        //public async Task LateDates()
        //{
        //    _jobObservationsAux = await JobObservationService.GetAllJobObservations();

        //    foreach (var jobobs in _jobObservationsAux)
        //    {
        //        if (Convert.ToDateTime(jobobs.EndDate?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 6 && jobobs.Status != 3 && jobobs.Status != 7)
        //        {
        //            jobobs.Status = 3;

        //            await JobObservationService.UpdateJobObservation(jobobs, "S.M. System");
        //        }
        //    }
        //}


        async Task DeleteJobObservation(int jobObservationId)
        {
            await JobObservationService.DeleteJobObservation(jobObservationId);
            ClearFilters();

            await SelectTableEvent0.ReloadServerData();

            visibleDelete = false;

            StateHasChanged();
        }

        IDialogReference dialogDate;
        private async void OpenCommentDialog()
        {
            var parameters = new DialogParameters { { "_jobObservation", _jobObservation }, { "ChangeDate", EventCallback.Factory.Create(this, ChangeDate) } };
            dialogDate = await DialogService.ShowAsync<ChangeDate_Dialog>("", parameters, dialogCommentOptions);
            await dialogDate.Result;
        }
        private DialogOptions dialogCommentOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        public string hour1 { get; set; }
        public string hour2 { get; set; }

        DateTime newDate1;
        DateTime newDate2;
        public async Task ChangeDate()
        {
            if (_jobObservation.Justification == null || _jobObservation.Justification == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["AddComment"], Severity.Error);
                return;
            }

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateError"], Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                Console.WriteLine(_jobObservation.StartDate);
                Console.WriteLine(_jobObservation.EndDate);


                if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["YouNeedChangeDate"], Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);

                    dialogDate.Close();
                    visible = false;

                    StateHasChanged();

                    await SelectTableEvent0.ReloadServerData();
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
            else
            {
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                Console.WriteLine(_jobObservation.StartDate);
                Console.WriteLine(_jobObservation.EndDate);


                if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["YouNeedChangeDate"], Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);


                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);

                    dialogDate.Close();
                    visible = false;

                    StateHasChanged();

                    await SelectTableEvent0.ReloadServerData();
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }

        void EditJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        void CreateJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);

                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createjobobservation/{dateString}");
            }
        }

        void CreateNewJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);

                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createnewjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createnewjobobservation/{dateString}");
            }
        }

        void PlanJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);
                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/planjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/planjobobservation/{dateString}");
            }
        }

        public bool flagJob = false;
        private bool visible = false;
        private int jobId;
        JobObservation _jobObservation = null;
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }
        DateTime? plannedStartDate = new();
        DateTime? plannedEndDate = new();
        private async void OpenDialog2(int id)
        {
            jobId = id;
            _jobObservation = null;
            _ = Task.Run(async () =>
            {
                // Obtener JobObservation en segundo plano
                _jobObservation = await JobObservationService.GetJobObservationById(jobId, true, true, true, includeCkAnswers: true);

                // Ejecutar las asignaciones una vez que se obtenga la observación del trabajo
                startHour = _jobObservation.StartDate?.TimeOfDay;
                endHour = _jobObservation.EndDate?.TimeOfDay;

                if (_jobObservation.PlannedStartDate != null)
                {
                    plannedStartDate = _jobObservation.PlannedStartDate;
                    plannedEndDate = _jobObservation.PlannedEndDate;
                }
                else
                {
                    plannedStartDate = _jobObservation.StartDate;
                    plannedEndDate = _jobObservation.EndDate;
                }

                // Aquí podrías forzar la actualización de la interfaz si es necesario
                InvokeAsync(StateHasChanged);
            });

            visible = true;
        }
        void Close() => visible = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        private bool FilterFunc(JobObservation element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.JobObservationId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Distribution.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operations.FirstOrDefault().Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.StartDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operator.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.JobObservationId} {element.Supervisor.Name} {element.Operator}".Contains(searchString))
                return true;
            return false;
        }

        public void ShowError()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Select a Distribution first", Severity.Error);
        }

        //Delete Job observation
        private bool visibleDelete = false;
        public int deleteJobObservationId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteJobObservationId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        //Double clic go to details
        private DateTime lastTouchTime = DateTime.MinValue;
        private readonly TimeSpan doubleTouchInterval = TimeSpan.FromMilliseconds(300);

        private void HandleTouchStart(int jobObsId)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastTouch = now - lastTouchTime;

            if (timeSinceLastTouch < doubleTouchInterval)
            {
                OpenDialog2(jobObsId);
            }

            lastTouchTime = now;
        }

        private int selectedRowNumber = -1;
        private MudTable<JobObservation> SelectTableEvent0;
        private MudTable<JobObservation> SelectTableEvent1;
        private MudTable<JobObservation> SelectTableEvent2;
        private MudTable<JobObservation> SelectTableEvent3;
        private MudTable<JobObservation> SelectTableEvent4;
        private MudTable<JobObservation> SelectTableEvent5;
        private MudTable<JobObservation> SelectTableEvent6;
        private MudTable<JobObservation> SelectTableEvent7;
        private MudTable<JobObservation> SelectTableEvent8;


        private void RowClickEvent(TableRowClickEventArgs<JobObservation> args, int tableId)
        {
            List<JobObservation> filteredItems = _jobObservations.ToList();
            //switch (tableId)
            //{
            //    case 0:
            //        filteredItems = SelectTableEvent0.Items.ToList();
            //        break;
            //    case 1:
            //        filteredItems = SelectTableEvent1.Items.ToList();
            //        break;
            //    case 2:
            //        filteredItems = SelectTableEvent2.Items.ToList();
            //        break;
            //    case 3:
            //        filteredItems = SelectTableEvent3.Items.ToList();
            //        break;
            //    case 4:
            //        filteredItems = SelectTableEvent4.Items.ToList();
            //        break;
            //    case 5:
            //        filteredItems = SelectTableEvent5.Items.ToList();
            //        break;
            //    case 6:
            //        filteredItems = SelectTableEvent6.Items.ToList();
            //        break;
            //    case 7:
            //        filteredItems = SelectTableEvent7.Items.ToList();
            //        break;
            //    case 8:
            //        filteredItems = SelectTableEvent8.Items.ToList();
            //        break;
            //}
            // Obtiene el índice dentro del subconjunto filtrado
            var rowIndex = filteredItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
                // Si la fila ya está seleccionada, se abre el diálogo (doble clic simulado)

                if (args.Item.Type == 3 && args.Item.Status == 7)
                {
                    OpenDialog3(args.Item.JobObservationId);
                }
                else
                {
                    OpenDialog2(args.Item.JobObservationId);
                }
            }
            else
            {
                // Cambia la fila seleccionada en el subconjunto filtrado
                selectedRowNumber = rowIndex;
                //SelectTableEvent0.SelectedItem = args.Item; // Actualiza la selección de la tabla

                switch (tableId)
                {
                    case 0:
                        SelectTableEvent0.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 1:
                        SelectTableEvent1.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 2:
                        SelectTableEvent2.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 3:
                        SelectTableEvent3.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 4:
                        SelectTableEvent4.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 5:
                        SelectTableEvent5.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 6:
                        SelectTableEvent6.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 7:
                        SelectTableEvent7.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                    case 8:
                        SelectTableEvent8.SelectedItem = args.Item; // Actualiza la selección de la tabla
                        break;
                }
                StateHasChanged(); // Actualiza la UI
            }
        }

        private string SelectedRowClassFunc(JobObservation element, int rowNumber, int tableId)
        {
            if (_jobObservations.Count() > 0)
            {
                List<JobObservation> filteredItems = _jobObservations.ToList();
                //switch (tableId)
                //{
                //    case 0:
                //        filteredItems = SelectTableEvent0.Items.ToList();
                //        break;
                //    case 1:
                //        filteredItems = SelectTableEvent1.Items.ToList();
                //        break;
                //    case 2:
                //        filteredItems = SelectTableEvent2.Items.ToList();
                //        break;
                //    case 3:
                //        filteredItems = SelectTableEvent3.Items.ToList();
                //        break;
                //    case 4:
                //        filteredItems = SelectTableEvent4.Items.ToList();
                //        break;
                //    case 5:
                //        filteredItems = SelectTableEvent5.Items.ToList();
                //        break;
                //    case 6:
                //        filteredItems = SelectTableEvent6.Items.ToList();
                //        break;
                //    case 7:
                //        filteredItems = SelectTableEvent7.Items.ToList();
                //        break;
                //    case 8:
                //        filteredItems = SelectTableEvent8.Items.ToList();
                //        break;
                //}

                // Solo devuelve la clase "selected" si el número de fila coincide con el seleccionado en el subconjunto filtrado
                if (filteredItems.IndexOf(element) == selectedRowNumber)
                {
                    return "selected";
                }
            }
            return string.Empty;
        }



        public string GetStatusLabel(int status)
        {
            return status switch
            {
                1 => "planned",
                2 => "inProgress",
                3 => "late",
                4 => "underReview",
                5 => "rejected",
                6 => "finished",
                _ => "",
            };
        }

        public string programmedStartDate = "";
        private bool visible2 = false;
        private int jobId2;
        private void OpenDialog3(int id)
        {
            //aquii me dio erro rde render
            DateTime date = _jobObservations.Find(j => j.JobObservationId == id).PlannedStartDate.Value;
            string fechaComoString = date.ToString("MM-dd-yyyy");

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = date;

                var EnglishStartDate = formatedStartDate.Month.ToString() + "/" + formatedStartDate.Day.ToString() + "/" + formatedStartDate.Year.ToString();
                var formatedStartDate2 = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                programmedStartDate = formatedStartDate2.ToString("MM-dd-yyyy");
            }
            else
            {
                var hour1 = date.ToShortDateString();

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy", null, DateTimeStyles.None, out var newDate1))
                {
                    //Console.WriteLine(newDate1);
                }
                else
                    //Console.WriteLine("Unable to parse {es-ES} '{0}'", hour1);

                    programmedStartDate = newDate1.ToString("MM-dd-yyyy");
            }
            jobId2 = id;
            visible2 = true;
        }
        void Close2() => visible2 = false;
        MudTabs JobTabs;
        private async Task HandleVisibleChanged(bool newValue)
        {
            showLoading = true;
            StateHasChanged();
            visible2 = newValue;

            if (!visible2)
                JobTabs.ActivatePanel(2);

            showLoading = false;
            StateHasChanged();
        }


        IDialogReference dialogOperations;
        private DialogOptions dialogOperationsOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        private async void OpenChangeOperations()
        {
            _operations = await OperationServices.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            var parameters = new DialogParameters { { "_jobObservation", _jobObservation }, { "ChangeOperations", EventCallback.Factory.Create(this, ChangeOperations) } };
            parameters.Add("_operations", _operations);
            dialogOperations = await DialogService.ShowAsync<ChangeOperations_Dialog>("", parameters, dialogOperationsOptions);
            await dialogOperations.Result;
        }


        public async Task ChangeOperations()
        {
            //if (_jobObservation.Justification == null || _jobObservation.Justification == "")
            //{
            //    Snackbar.Clear();
            //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //    Snackbar.Add(Localizer["AddComment"], Severity.Error);
            //    return;
            //}

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
               

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["OperationsChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);

                    dialogOperations.Close();
                    visible = false;

                    StateHasChanged();

                    await SelectTableEvent0.ReloadServerData();
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
            else
            {
                

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);


                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["OperationsChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);

                    dialogOperations.Close();
                    visible = false;

                    StateHasChanged();

                    await SelectTableEvent0.ReloadServerData();
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }


    }//end class 
}//end namespace    
