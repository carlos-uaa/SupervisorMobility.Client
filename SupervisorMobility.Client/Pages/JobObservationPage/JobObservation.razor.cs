using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class JobObservation
    {

        public DateTime? date = DateTime.Today;
        DateTime? _yearMonth = DateTime.Today;
        private string month;
        private string year;
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        public List<Group> _groups { get; set; } = new();

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
            "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
            "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        public int plantId;
        public int areaId;
        public int groupId;
        private string operacion { get; set; }
        public string operador { get; set; } = "Pedro";
        public string observador { get; set; } = "Juan";
        public string time1 { get; set; } = "10 min";
        public string anomaly { get; set; }
        public string time2 { get; set; } = "20 min";
        string[] models = new string[5] { "P71A", "X247", "P71A", "X247", "P71A" };
        string[] cicles = new string[5] { "1 min", "2 min", "3 min", "4 min", "5 min" };

        public string sArea;
        public string qArea;
        public string dArea;
        public string cArea;
        public string othersArea;
        public int option { get; set; } = 1;
        bool displayModal = false;
        List<string> monthNames = new List<string>();
        List<string> days = new List<string>();
        List<Week> weeks = new List<Week>();

        DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "", disabled: true)
        };

        DisplayNameLabelClass model = new();

        public class DisplayNameLabelClass
        {
            public DateTime? Date { get; set; }
            public bool Boolean { get; set; }
            public string String { get; set; }
        }
        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
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
        }

        private async void ShowAreas()
        {
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

       


    }
}
