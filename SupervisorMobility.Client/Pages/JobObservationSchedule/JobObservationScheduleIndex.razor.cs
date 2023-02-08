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

        public int plantId; 
        public int areaId;

        public string supervisor { get; set; } = "Pedro";

        private string proceso { get; set; }
        private string area { get; set; }
        private string grupo { get; set; }
        private string ssv { get; set; }


        bool displayModal = false;
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
            _plants = await PlantServices.GetPlants();
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "", disabled: true)
        };

        protected override void OnInitialized()
        {
            monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames.ToList();
            GenerateCalendarHead();
            GenerateCalendarBody();
        }
        private void GenerateCalendarHead()
        {
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
                    DateValue = dt.ToString("dd-MM-yyyy"),
                    DayName = dt.ToString("dddd")
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

        private void CloseModal()
        {
            displayModal = false;
        }

        private void OpenModal(int wIndex, int dIndex)
        {
            displayModal = true;
        }


        private void OpenDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogService.Show<DialogExample>("Simple Dialog", options);
        }

        private async void ShowAreas()
        {
            _areas = await AreaServices.GetAreas(plantId);
        }

        private async void ShowGroups()
        {
            _groups = await GroupService.GetGroups();
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

    }

}
