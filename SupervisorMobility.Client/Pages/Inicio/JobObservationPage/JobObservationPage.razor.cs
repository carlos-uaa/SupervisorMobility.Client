using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class JobObservationPage
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

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

        //Filters
        public List<JobObservation> _filterJobObservation { get; set; } = new();



        //Job observations status lists.
        public List<JobObservation> _jobObservations { get; set; } = new();
        public List<JobObservation> _jobObservationsAux { get; set; } = new();


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


        public int plantId;
        public int areaId;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();



        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation", disabled: true),
            };

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

                await GetUserAsync();
                //await LateDates();
                _jobObservations.Clear();
                JobObservationsTotalCount();
                ClearFilters();

                if (user != null)
                {
                    if(user.UserType != 3)
                    {
                        _jobObservationsAux = await JobObservationService.GetAllJobObservations(true, true);
                    }
                    else
                    {
                        _jobObservationsAux = await JobObservationService.GetAllJobObservations(true, true, idUser: user.UserId);
                    }

                    if (user.UserType == 1 || user.UserType == 6)
                    {
                        _plants = await PlantServices.GetPlants();
                        _plants = _plants.OrderBy(p => p.Description).ToList();

                        foreach (var jobobs in _jobObservationsAux)
                        {

                            _jobObservations.Add(jobobs);

                        }

                    }
                    else if (user.UserType == 2)
                    {
                        plantId = (int)user.PlantId;

                        if (user.Areas != null)
                        {
                            _areas = user.Areas.ToList();
                            _areas.OrderBy(a => a.Description).ToList();
                        }
                        foreach (var jobobs in _jobObservationsAux)
                        {
                            if (plantId == jobobs.PlantId)
                            {
                                foreach (User usr in user.Subordinates)
                                {
                                    if (jobobs.SupervisorId == usr.UserId && jobobs.PlantId == plantId)
                                    {
                                        _jobObservations.Add(jobobs);

                                    }
                                }
                            }
                        }

                    }

                    else if (user.UserType == 3)
                    {
                        plantId = (int)user.PlantId;
                        areaId = (int)user.AreaId;

                        _jobObservations = _jobObservationsAux.ToList();

                        _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);

                        operatorUsers = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, true, false);
                    }
                    else if (user.UserType == 5)
                    {
                        _plants = await PlantServices.GetPlants();
                        _plants = _plants.OrderBy(p => p.Description).ToList();

                        plantId = (int)user.PlantId;
                        _areas = await AreaServices.GetAreas(plantId);
                        _areas = _areas.OrderBy(a => a.Description).ToList();

                        foreach (var jobobs in _jobObservationsAux)
                        {
                            if (plantId == jobobs.PlantId)
                            {
                                foreach (User usr in user.Subordinates)
                                {
                                    if (jobobs.Supervisor.SuperiorId == usr.UserId)
                                    {

                                        _jobObservations.Add(jobobs);

                                    }

                                }
                            }
                        }
                    }

                    _filterJobObservation = _jobObservations;
                    JobObservationsTotalCount();
                    StateHasChanged();
                }
            }
        }


        private void JobObservationsTotalCount()
        {
            totalJobObservations = Localizer["allJobObservations"] + " (" + _jobObservations.Where(j => j.Status != 7).Count() + ")";
            totalPlanned = Localizer["planned"] + " (" + _jobObservations.Where(j => j.Status == 1).Count() + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _jobObservations.Where(j => j.Status == 2).Count() + ")";
            totalLate = Localizer["late"] + " (" + _jobObservations.Where(j => j.Status == 3).Count() + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _jobObservations.Where(j => j.Status == 4).Count() + ")";
            totalRejected = Localizer["rejected"] + " (" + _jobObservations.Where(j => j.Status == 5).Count() + ")";
            totalFinished = Localizer["finished"] + " (" + _jobObservations.Where(j => j.Status == 6).Count() + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _jobObservations.Where(j => j.Status == 7).Count() + ")";
        }


        private async void ShowAreas()
        {

            if (plantId == 0)
            {
                areaId = 0;
                ClearFilters();
                _jobObservations = new();
                foreach (var jobobs in _jobObservationsAux)
                {

                    _jobObservations.Add(jobobs);

                }
                _filterJobObservation = _jobObservations;
                JobObservationsTotalCount();
                StateHasChanged();
                return;

            }

            areaId = 0;
            color = Color.Default;
            filters = false;
            ClearFilters();
            operatorUsers.Clear();
            _jobObservations.Clear();


            foreach (var jobobs in _jobObservationsAux)
            {
                if (plantId == jobobs.PlantId)
                {

                    _jobObservations.Add(jobobs);

                }
            }

            _filterJobObservation = _jobObservations;
            JobObservationsTotalCount();


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
            _jobObservations.Clear();
            operatorUsers.Clear();


            if (user.UserType == 1 || user.UserType == 6)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && areaId == jobobs.AreaId)
                    {

                        _jobObservations.Add(jobobs);

                    }
                }
            }
            else if (user.UserType == 2)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.SupervisorId == usr.UserId && usr.AreaId == areaId)
                            {

                                _jobObservations.Add(jobobs);

                            }
                        }
                    }
                }
            }
            else if (user.UserType == 3)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && jobobs.SupervisorId == user.UserId && user.AreaId == areaId)
                    {

                        _jobObservations.Add(jobobs);

                    }
                }
            }
            else if (user.UserType == 5)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.Supervisor.SuperiorId == usr.UserId && jobobs.AreaId == areaId)
                            {

                                _jobObservations.Add(jobobs);

                            }
                        }
                    }
                }
            }

            _filterJobObservation = _jobObservations;
            JobObservationsTotalCount();

            _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);

            //operator User
            //users = await UsersService.GetUsers();
            //foreach (var operatorUser in users)
            //{
            //    if (operatorUser.PlantId == plantId && operatorUser.AreaId == areaId && operatorUser.UserType == 4)
            //    {
            //        operatorUsers.Add(operatorUser);
            //    }
            //}
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

            _jobObservations = _filterJobObservation;
            JobObservationsTotalCount();

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();

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
            _jobObservations = _filterJobObservation;

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();

            StateHasChanged();
        }

        //Filters
        private void FilterDistributions()
        {

            if(distributionId == 0)
            {
                _jobObservations = _filterJobObservation;
                return;
            }

            operationId = new();
            _jobObservations = _filterJobObservation;


            operationFlag = true;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == distributionId)].Operations;


            _jobObservations = _jobObservations.Where(jobObs => jobObs.DistributionId == distributionId).ToList();

            if (statusId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.Status == statusId).ToList();

            }
            if (operatorId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.OperatorId == operatorId).ToList();

            }
            if (filterDate != null)
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();

            }
            if (idFilter != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();

            }
            JobObservationsTotalCount();

        }

        private void Filters()
        {
            _jobObservations = _filterJobObservation;


            if (distributionId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.DistributionId == distributionId).ToList();

            }
            if (operationId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.OperationId == operationId).ToList();

            }
            if (statusId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.Status == statusId).ToList();

            }
            if (operatorId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.OperatorId == operatorId).ToList();

            }
            if (filterDate != null)
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();

            }
            if (idFilter != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();

            }

            JobObservationsTotalCount();
        }

        public void ClearStatus()
        {
            statusId = new();
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
            _jobObservations.RemoveAll(jobObservation => jobObservation.JobObservationId == jobObservationId);
            await JobObservationService.DeleteJobObservation(jobObservationId);
            ClearFilters();


            if (user.UserType == 1)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && areaId == jobobs.AreaId)
                    {

                        _jobObservations.Add(jobobs);


                    }
                }
            }
            else if (user.UserType == 2)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.SupervisorId == usr.UserId && usr.AreaId == areaId)
                            {

                                _jobObservations.Add(jobobs);

                            }
                        }
                    }
                }
            }
            else if (user.UserType == 3)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && jobobs.SupervisorId == user.UserId && user.AreaId == areaId)
                    {

                        _jobObservations.Add(jobobs);

                    }
                }
            }
            else if (user.UserType == 5)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.Supervisor.SuperiorId == usr.UserId && jobobs.AreaId == areaId)
                            {

                                _jobObservations.Add(jobobs);

                            }
                        }
                    }
                }
            }

            JobObservationsTotalCount();

            visibleDelete = false;

            StateHasChanged();
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
        private void OpenDialog2(int id)
        {
            jobId = id;
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
            if (element.Operation.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
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
        private MudTable<JobObservation> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<JobObservation> tableRowClickEventArgs)
        {
        }


        private string SelectedRowClassFunc(JobObservation element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
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
    }
}
