using AutoMapper;
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

        public string supervisor { get; set; } = "Pedro";

        private string proceso { get; set; }
        private string area { get; set; }
        private string grupo { get; set; }
        private string ssv { get; set; } = "Azael";

        List<string> monthNames = new List<string>();
        List<string> days = new List<string>();
        List<Week> weeks = new List<Week>();

        DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);


        DisplayNameLabelClass model = new();





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

                    _jobObservations = _allJobObservations;

                    _plants = await PlantServices.GetPlants();
                    _groups = await GroupService.GetGroups();

                    if(user.UserType == 1)
                    {
                        plantId = 0;
                        areaId = 0;
                        groupId = 0;
                        ssvId = 0;
                        supervisorId = 0;
                        _allSSVs = await UsersService.GetUserByTypeAndCollection(2);
                        _allSupervisors = await UsersService.GetUserByTypeAndCollection(3);
                    }
                    else if(user.UserType == 2)
                    {
                        plantId = (int)user.PlantId;
                        areaId = 0;
                        groupId = (int)user.GroupId;
                        ssv = user.Name;
                        ssvId = user.UserId;
                        supervisorId = 0;
                        _allSupervisors = await UsersService.GetUserByTypeAndCollection(3);
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

                        _jobObservations = new();
                        foreach (var jobobs in _allJobObservations)
                        {
                            if (jobobs.SupervisorId == user.UserId && user.AreaId == areaId)
                            {
                                _jobObservations.Add(jobobs);
                            }
                        }
                    }
                    else if(user.UserType == 5)
                    {
                        plantId = (int)user.PlantId;
                        areaId = 0;
                        groupId = 0;
                        ssvId = 0;
                        supervisorId = 0;
                        _areas = await AreaServices.GetAreas(plantId);
                        _allSSVs = await UsersService.GetUserByTypeAndCollection(2);
                        _allSupervisors = await UsersService.GetUserByTypeAndCollection(3);
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
            //_allJobObservations = await JobObservationService.GetAllJobObservations();

            //foreach (var jobobs in _allJobObservations)
            //{
            //    if (Convert.ToDateTime(jobobs.EndDate?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 6 && jobobs.Status != 3)
            //    {
            //        jobobs.Status = 3;

            //        await JobObservationService.UpdateJobObservation(_mapper.Map<JobObservation>(jobobs), "S.M. System");
            //    }
            //}
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

            StateHasChanged();

        }

        private void ShowSSV()
        {


            if (user.UserType == 1)
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

            StateHasChanged();
        }

        private void ShowSupervisors()
        {

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

                foreach (User sv in _allSupervisors)
                {
                    if (sv.SuperiorId == ssvId && sv.AreaId == areaId)
                        _supervisors.Add(sv);
                }

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

                foreach (User sv in _allSupervisors)
                {
                    if (sv.SuperiorId == ssvId && sv.AreaId == areaId)
                        _supervisors.Add(sv);
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

            }
            StateHasChanged();
        }

        private void JobObservationsBySupervisor()
        {
            _jobObservations = new();
            foreach (var jobobs in _allJobObservations)
            {
                if (jobobs.SupervisorId == supervisorId)
                {
                    _jobObservations.Add(jobobs);
                }
            }
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

    }

}
