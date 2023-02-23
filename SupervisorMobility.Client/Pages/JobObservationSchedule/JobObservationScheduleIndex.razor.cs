using Microsoft.JSInterop;
using MudBlazor;
using System;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.JobObservationSchedule
{
    public partial class JobObservationScheduleIndex
    {

        public DateTime? date = DateTime.Today;
        DateTime? _yearMonth = DateTime.Today;
        private string month;
        private string year;
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        public List<Group> _groups { get; set; } = new();
        public List<JobObservation> _jobObservation { get; set; } = new();



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

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _jobObservation = await JobObservationService.GetAllJobObservations();
            _plants = await PlantServices.GetPlants();
            _groups = await GroupService.GetGroups();
        }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation Schedule", href: "", disabled: true)
        };

        protected override void OnInitialized()
        {
            monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames.ToList();
            GenerateCalendarHead();
            GenerateCalendarBody();
        }
        private void GenerateCalendarHead()
        {
            if (startDate.DayOfWeek == DayOfWeek.Monday)
            {
                Console.Write("lunes");
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


        //private void OpenDialog()
        //{
        //    var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        //    DialogService.Show<DialogExample>("Simple Dialog", options);
        //}


        private async void ShowAreas()
        {
            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
        }

        private void OnDateChanged(DateTime? value)
        {
            _yearMonth = value;
            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            Console.WriteLine(_yearMonth);
            Console.WriteLine(DateTime.Now.Year.ToString());
            Console.WriteLine(year);
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);

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
