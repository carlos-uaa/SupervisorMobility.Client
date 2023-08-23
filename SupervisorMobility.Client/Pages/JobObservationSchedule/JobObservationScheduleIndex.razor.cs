using AutoMapper;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Builder;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.JobObservationSchedule
{
    public partial class JobObservationScheduleIndex
    {

        public DateTime? date = DateTime.Today;
        DateTime? _yearMonth = DateTime.Today;
        private string month;
        private string showMonth;
        private string year;
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        public List<Group> _groups { get; set; } = new();

        public List<JobObservation> _allJobObservations { get; set; } = new();
        public List<JobObservation> _jobObservations { get; set; } = new();

        public List<JobObservation> _SOSJobobservation { get; set; } = new();
        public List<JobObservation> _DayJobObservations { get; set; } = new();

        private readonly IMapper _mapper;

        List<User> _allSSVs = new();
        List<User> _SSVs = new();
        List<User> _supervisors = new();
        List<User> _allSupervisors = new();
        public int ssvId;
        public int supervisorId;

        public string SupervisorName = string.Empty;


        public int plantId;
        public int areaId;
        public int groupId;
        public bool displayInfo { get; set; }
        public string supervisor { get; set; } = "Pedro";
        private string ssv { get; set; } = "Azael";

        List<string> monthNames = new List<string>();
        List<string> days = new List<string>();
        List<Week> weeks = new List<Week>();

        DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);


        DisplayNameLabelClass model = new();


        public int totalProgrammed;


        public class DisplayNameLabelClass
        {
            public DateTime? Date { get; set; }
            public bool Boolean { get; set; }
            public string String { get; set; }
        }

        public int optionStatus { get; set; } = 0;

        //User
        private string json = string.Empty;
        public User user = new();


        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            displayInfo = true;
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem(text: Localizer["jobObservationSchedule"], href: "", disabled: true)
            };

            if (!await HasPropertyAsync())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["errorYouHaveToLogIn"], Severity.Warning);
                NavigationManager.NavigateTo($"/");
            }
            else
            {

                await GetUserAsync();
                await LateDates();
                monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames.ToList();
                GenerateCalendarHead();
                GenerateCalendarBody();

                if (user != null)
                {
                    _allJobObservations = await JobObservationService.GetAllJobObservations();


                    _plants = await PlantServices.GetPlants();
                    _groups = await GroupService.GetGroups();

                    if(user.UserType == 1)
                    {
                        plantId = 0;
                        areaId = 0;
                        groupId = 0;
                        ssvId = 0;
                        supervisorId = 0;
                        _allSSVs = await UsersService.GetUsersByType(2, true, true);

                        _jobObservations = _allJobObservations;

                        _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
                        totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
                    }
                    else if(user.UserType == 2)
                    {
                        plantId = (int)user.PlantId;
                        areaId = 0;
                        groupId = (int)user.GroupId;
                        ssv = user.Name;
                        ssvId = user.UserId;
                        supervisorId = 0;

                        _allSupervisors = user.Subordinates?.ToList();

                        foreach(var jobobs in _allJobObservations)
                        {
                            if(jobobs.Supervisor.SuperiorId == user.UserId && jobobs.PlantId == plantId)
                            {
                                _jobObservations.Add(jobobs);
                            }
                        }

                        _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
                        totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
                    }
                    else if(user.UserType == 3)
                    {

                        plantId = (int)user.PlantId;
                        areaId = (int)user.AreaId;
                        groupId = (int)user.GroupId;
                        _areas = await AreaServices.GetAreas(plantId);
                        supervisor = user.Name;
                        supervisorId = user.UserId;
                        ssv = user.Superior.Name;
                        ssvId = (int)user.SuperiorId;


                        foreach (var jobobs in _allJobObservations)
                        {
                            if (jobobs.SupervisorId == user.UserId && user.AreaId == areaId)
                            {
                                _jobObservations.Add(jobobs);
                            }
                        }

                        _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
                        totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();

                    }
                    else if(user.UserType == 5)
                    {
                        plantId = (int)user.PlantId;
                        areaId = 0;
                        groupId = 0;
                        ssvId = 0;
                        supervisorId = 0;
                        _areas = await AreaServices.GetAreas(plantId);
                        _allSSVs = await UsersService.GetUsersByType(2, true, true);

                        foreach (var jobobs in _allJobObservations)
                        {
                            if (plantId == jobobs.PlantId)
                            {
                                foreach (User usr in user.Subordinates)
                                {
                                    if (jobobs.Supervisor.SuperiorId == usr.UserId)
                                    {
                                        if (jobobs.Status != 7)
                                        {
                                            _jobObservations.Add(jobobs);
                                        }
                                    }

                                }
                            }
                        }

                        _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
                        totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
                        //_allSupervisors = await UsersService.GetUserByType(3, true, false);
                    }
                    else if (user.UserType == 6)
                    {
                        plantId = (int)user.PlantId;
                        areaId = 0;
                        groupId = (int)user.GroupId;
                        ssvId = 0;
                        supervisorId = 0;

                        _allSSVs = await UsersService.GetUsersByType(2, true, true);
                        
                        foreach (var jobobs in _allJobObservations)
                        {
                            if (jobobs.PlantId == plantId)
                            {
                                _jobObservations.Add(jobobs);
                            }
                        }
                        _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
                        totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
                    }
                }
                    StateHasChanged();
            }
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


        //Change the status if the observation is late
        public async Task LateDates()
        {
            _allJobObservations = await JobObservationService.GetAllJobObservations();

            foreach (var jobobs in _allJobObservations)
            {
                if (Convert.ToDateTime(jobobs.EndDate?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 6 && jobobs.Status != 3 && jobobs.Status != 7)
                {
                    jobobs.Status = 3;

                    await JobObservationService.UpdateJobObservation(jobobs, "S.M. System");
                }
            }
        }


        private async Task HandleVisibleChanged(bool newValue)
        {
            plantId = 0;
            ssvId = 0;
            _SSVs.Clear();

            supervisorId = 0;
            _supervisors.Clear();

            _supervisors.Clear();
            supervisorId = 0;

            areaId = 0;
            _allJobObservations = await JobObservationService.GetAllJobObservations();

            _jobObservations = _allJobObservations;

            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            visible2 = newValue;
            StateHasChanged();

        }

        public async Task LastMonth()
        {
            _yearMonth = _yearMonth?.AddMonths(-1);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);

            showMonth = month.ToUpper();
            GenerateCalendarHead();
            GenerateCalendarBody();
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();

            StateHasChanged();
        }

        public async Task NextMonth()
        {
            _yearMonth = _yearMonth?.AddMonths(1);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);

            showMonth = month.ToUpper();
            GenerateCalendarHead();
            GenerateCalendarBody();
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();

            StateHasChanged();
        }
            private void GenerateCalendarHead()
        {
            if (startDate.DayOfWeek == DayOfWeek.Monday)
            {
                startDate = startDate.AddDays(-1);
            }
            switch (startDate.DayOfWeek)
            {
                case DayOfWeek.Monday: startDate = startDate.AddDays(-1); break;
                case DayOfWeek.Tuesday: startDate = startDate.AddDays(-2); break;
                case DayOfWeek.Wednesday: startDate = startDate.AddDays(-3); break;
                case DayOfWeek.Thursday: startDate = startDate.AddDays(-4); break;
                case DayOfWeek.Friday: startDate = startDate.AddDays(-5); break;
                case DayOfWeek.Saturday: startDate = startDate.AddDays(-6); break;


            }

            var day1 = new List<string>();
            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                day1.Add(dt.ToString("dddd"));
            }
            days = day1.Distinct().ToList();
        }
        private void GenerateCalendarBody()
        {
            weeks = new List<Week>();
            int flag = 0;
            Week week = new Week();
            List<DayEvent> dates = new List<DayEvent>();

            var totalDays = (int)(endDate - startDate).TotalDays;
            int countdays = 0;

            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                flag = flag + 1;
                dates.Add(new DayEvent()
                {
                    DateValue = dt.ToString("d/M/yyyy"),
                    DayName = dt.ToString("dd")
                });

                if (flag == 7)
                {
                    week = new Week();
                    week.Dates = dates;
                    weeks.Add(week);
                    dates = new List<DayEvent>();
                    flag = 0;
                }
                if (countdays == totalDays)
                {
                    week = new Week();
                    week.Dates = dates;
                    weeks.Add(week);
                    break;
                }
                countdays++;
            }
        }

        public void Help()
        {

        }

        private void CreateJobObservation(string date)
        {
            date = date.Replace("/", "-");
            NavigationManager.NavigateTo($"jobobservation/createjobobservation/{date}");
        }

        void PlanJobObservation(string date)
        {
            date = date.Replace("/", "-");
            NavigationManager.NavigateTo($"jobobservation/planjobobservation/{date}");
        }

        //private void OpenDialog()
        //{
        //    var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        //    DialogService.Show<DialogExample>("Simple Dialog", options);
        //}


        private async void ShowAreas()
        {
            if(plantId == 0 && user.UserType == 1)
            {
                areaId = 0;
                groupId = 0;
                ssvId = 0;
                 
                supervisorId = 0;

                _jobObservations = new();
                _jobObservations = _allJobObservations;
                _SSVs.Clear();
                _supervisors.Clear();
                _supervisors.Clear();
                StateHasChanged();
                return;
            }

            _jobObservations = new();
            foreach(var jobobs in _allJobObservations)
            {
                if(jobobs.PlantId == plantId)
                {
                    _jobObservations.Add(jobobs);
                }
            }


            ssvId = 0;
            _SSVs.Clear();

            supervisorId = 0;
            _supervisors.Clear();

            _supervisors.Clear();
            supervisorId = 0;

            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            StateHasChanged();

        }

        private void ShowSSV()
        {
            if(areaId == 0)
            {
                ShowAreas();
                return;
            }


            if (user.UserType == 1 || user.UserType == 6)
            {

                supervisorId = 0;
                _supervisors.Clear();

                _SSVs.Clear();
                ssvId = 0;


                foreach (User ssv in _allSSVs)
                {
                    if (ssv.PlantId == plantId && ssv.Areas?.ToList().FindIndex(a => a.AreaId == areaId) != -1)
                    {
                        _SSVs.Add(ssv);
                    }
                }

                _jobObservations = new();
                foreach (var jobobs in _allJobObservations)
                {
                    if (jobobs.PlantId == plantId && jobobs.AreaId == areaId)
                    {
                        _jobObservations.Add(jobobs);
                    }
                }


            }
            else if(user.UserType == 2)
            {

                supervisorId = 0;
                _supervisors.Clear();

                foreach (User sv in _allSupervisors)
                {
                    if (sv.SuperiorId == ssvId && sv.AreaId == areaId)
                        _supervisors.Add(sv);
                }


                _jobObservations = new();
                foreach (var jobobs in _allJobObservations)
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
            else if(user.UserType == 5)
            {
                supervisorId = 0;
                _supervisors.Clear();

                _SSVs.Clear();
                ssvId = 0;


                foreach (User ssv in _allSSVs)
                {
                    if (ssv.PlantId == plantId && ssv.Areas?.ToList().FindIndex(a => a.AreaId == areaId) != -1)
                    {
                        _SSVs.Add(ssv);
                    }
                }

                _jobObservations = new();
                foreach (var jobobs in _allJobObservations)
                {
                    if (jobobs.PlantId == plantId && jobobs.AreaId == areaId)
                    {
                        _jobObservations.Add(jobobs);
                    }
                }
            }
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            StateHasChanged();
        }

        private void ShowSupervisors()
        {
            if(ssvId == 0)
            {
                ShowSSV();
                return;
            }


            _supervisors.Clear();
            supervisorId = 0;
            
            if(user.UserType != 5)
            {
                User SeniorSV = new();
                foreach(User usr in _allSSVs)
                {
                    if(usr.UserId == ssvId)
                    {
                        SeniorSV = usr;
                    }
                }


                _jobObservations = new();
                foreach (var jobobs in _allJobObservations)
                {
                    foreach(User usr in SeniorSV.Subordinates)
                    {
                        if (jobobs.SupervisorId == usr.UserId && usr.AreaId == areaId)
                        {
                            _jobObservations.Add(jobobs);
                        }
                    }
                }

                _supervisors = _allSSVs.Find(ssv => ssv.UserId == ssvId).Subordinates.ToList();
                _supervisors = _supervisors.Where(sv => sv.AreaId == areaId).ToList();

                //foreach (User sv in _allSupervisors)
                //{
                //    if (sv.SuperiorId == ssvId && sv.AreaId == areaId)
                //        _supervisors.Add(sv);
                //}

            }
            else
            {
                User SeniorSV = new();
                foreach(User usr in _allSSVs)
                {
                    if(usr.UserId == ssvId)
                    {
                        SeniorSV = usr;
                    }
                }

                _supervisors = _allSSVs.Find(ssv => ssv.UserId == ssvId).Subordinates.ToList();
                _supervisors = _supervisors.Where(sv => sv.AreaId == areaId).ToList();
                //foreach (User sv in _allSupervisors)
                //{
                //    if (sv.SuperiorId == ssvId && sv.AreaId == areaId)
                //        _supervisors.Add(sv);
                //}


                _jobObservations = new();
                foreach (var jobobs in _allJobObservations)
                {

                    foreach(User usr in SeniorSV.Subordinates)
                    {
                        if (jobobs.SupervisorId == usr.UserId && usr.AreaId == areaId)
                        {
                            _jobObservations.Add(jobobs);
                        }
                    }
                }

            }
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            StateHasChanged();
        }

        private void JobObservationsBySupervisor()
        {

            if (supervisorId == 0)
            {
                ShowSupervisors();
                return;
            }
            _jobObservations = new();
            foreach (var jobobs in _allJobObservations)
            {
                if (jobobs.SupervisorId == supervisorId)
                {
                    _jobObservations.Add(jobobs);
                }
            }
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            StateHasChanged();
        }

        private void OnDateChanged(DateTime? value)
        {
            _yearMonth = value;
            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);

            showMonth = month.ToUpper();
            GenerateCalendarHead();
            GenerateCalendarBody();
            _SOSJobobservation = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).ToList();
            totalProgrammed = _jobObservations.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            StateHasChanged();
        }

        void JobObservationUpdate(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        private bool visible = false;
        private int jobId;
        private void OpenDialog2(int id)
        {
            jobId = id;
            visible = true;
        }
        void Close() => visible = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };


        //Programmed Job observation Modal (SOS)
        private bool visible2 = false;
        private int jobId2;
        private void OpenDialog3(int id)
        {
            jobId2 = id;
            visible2 = true;
        }
        void Close2() => visible2 = false;


        //Button Programmed Job observation Modal (SOS)
        private bool visible3 = false;

        public string programmedStartDate = "";
        private void OpenDialog4(string date)
        {
            searchString = "";
            date = date.Replace("/", "-");
            programmedStartDate = date;
            visible3 = true;
        }
        void Close3() => visible3 = false;


        private string searchString = "";

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


        //All Job observation Modal 
        private bool visible5 = false;

        private void OpenDialog5(string date)
        {
            searchString = "";
            var compare = DateTime.ParseExact(date, "d/M/yyyy", null);
            _DayJobObservations = _jobObservations.Where(j => Convert.ToDateTime(j.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(compare.ToShortDateString()).Date && Convert.ToDateTime(compare.ToShortDateString()).Date <= Convert.ToDateTime(j.EndDate?.ToShortDateString()).Date && j.Status != 7).ToList();
            visible5 = true;
        }
        void Close5() => visible5 = false;
    }


}
