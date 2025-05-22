using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Runtime.CompilerServices;
using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;

namespace SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.Components
{
    public partial class CalendarSuggest
    {
        [Parameter]
        public bool IsDialog { get; set; } = false; 
        [Parameter]
        public int OpDia { get; set; } = 2;
        [Parameter]
        public DateTime StartDay { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        [Parameter]
        public int SeparateDays { get; set; } = 1;
        [Parameter]
        public int OptionRandom { get; set; } = 0;
       
        [Parameter]
        public List<DistSelect> Dist_Manager { get; set; } = new List<DistSelect>();
        [Parameter]
        public EventCallback StartCreateSuggestion { get; set; }
        public string DistributionsInput { get; set; } = "1,2";

        public int Year { get; set; } = 0;

        private string currentRandomDistribution = null;
        // Distribuciones y colores
        private List<string> Distributions = new List<string> { "1", "2" };
        private Dictionary<string, string> DistributionColors = new Dictionary<string, string>();
        // Modelo para el calendario
        private List<MonthModel> CalendarMonths = new List<MonthModel>();
        public List<Holiday> holidays { get; set; }
        bool Dev_env { get; set; }
        protected override async void OnInitialized()
        {
            Year = StartDay.Year;
            holidays = await CalendarServices.GetHolidaysInService(Year);
            Dev_env = Environment.IsDevelopment();
            DistributionsInput = string.Join(",", Dist_Manager.Where(d => d.isSelected == true).Select(d => d.distribution.Description).ToList());
            Distributions = Dist_Manager.Where(d => d.isSelected == true).Select(d => d.distribution.Description).ToList();
            GenerateRandomColors();
            await GenerateCalendar();
        }


        private void GenerateRandomColors()
        {
            var random = new Random();
            DistributionColors = Distributions.ToDictionary(
                dist => dist,
                dist => $"hsl({random.Next(0, 360)}, {random.Next(70, 100)}%, {random.Next(70, 85)}%)"
            );
        }

        private string GetDayClasses(DayModel day)
        {
            var classes = new List<string> { "day-cell" };

            if (day.IsEmpty)
            {
                classes.Add("empty-day");
            }
            else if (day.IsWeekend)
            {
                classes.Add("weekend-day");
            }

            if (day.HasOperation)
            {
                classes.Add("operation-day");
            }

            return string.Join(" ", classes);
        }

        private string GetDayStyle(DayModel day)
        {
            if (day.HasOperation)
            {
                return $"background-color: {DistributionColors[day.Distribution]};";
            }
            return "";
        }
  
        private async Task<AsyncVoidMethodBuilder> GenerateCalendar()
        {
            CalendarMonths.Clear();

            var selectedDistributions = Dist_Manager.Where(d => d.isSelected).Select(d => d.distribution).ToList();

            await GenerateCalendar_Distributed(selectedDistributions);

            return new AsyncVoidMethodBuilder();
        }

        private async Task GenerateCalendar_Distributed(List<Distribution> selectedDistributions)
        {
            var distributionStates = selectedDistributions
                .Select(d => new DistributionState { Distribution = d })
                .ToList();

            var calendarMonths = new List<MonthModel>();
            int year = StartDay.Year;
            DateTime today = DateTime.Today;
            DateTime currentDate = (StartDay >= today) ? StartDay : today;
            string lastDistributionAssigned = null;

            // Generamos el calendario de todos los meses
            for (int i = 0; i < 12; i++)
            {
                DateTime monthStart = new DateTime(year, 1, 1).AddMonths(i);
                calendarMonths.Add(CreateMonthModel(monthStart));
            }

            while (distributionStates.Any(d => d.CurrentOperationIndex < d.Distribution.Operations.Count))
            {
                foreach (var distState in distributionStates)
                {
                    if (distState.CurrentOperationIndex >= distState.Distribution.Operations.Count)
                        continue;

                    if (await IsWeekend(currentDate) || await IsHoliday(currentDate))
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }

                    var operation = distState.Distribution.Operations.ElementAtOrDefault(distState.CurrentOperationIndex);
                    if (operation == null)
                    {
                        distState.CurrentOperationIndex = distState.Distribution.Operations.Count;
                        continue;
                    }

                    //Ahora asignamos en el calendario visual
                    var monthModel = calendarMonths.FirstOrDefault(m => m.Year == currentDate.Year &&
                                                                         m.Days.Any(d => d.Date.Month == currentDate.Month));
                    var dayModel = monthModel?.Days.FirstOrDefault(d => d.Date.Date == currentDate.Date);

                    if (dayModel != null && !dayModel.HasOperation)
                    {
                        dayModel.HasOperation = true;
                        dayModel.Distribution = distState.Distribution.Description;
                        dayModel.OperationText = $"Op {operation.OperationId}";
                    }

                    distState.CurrentOperationIndex++;
                    lastDistributionAssigned = distState.Distribution.Description;

                    // Avanzar días respetando separación
                    currentDate = await AddWorkingDays(currentDate, SeparateDays);
                }
            }

            CalendarMonths = calendarMonths;
        }
        private MonthModel CreateMonthModel(DateTime monthStart)
        {
            var monthModel = new MonthModel
            {
                MonthName = monthStart.ToString("MMMM"),
                Year = monthStart.Year,
                Days = new List<DayModel>()
            };

            int daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);

            var firstDay = new DateTime(monthStart.Year, monthStart.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Días vacíos al inicio
            int firstDayOfWeek = (int)firstDay.DayOfWeek;
            for (int i = 0; i < firstDayOfWeek; i++)
            {
                monthModel.Days.Add(new DayModel { IsEmpty = true });
            }

            for (int day = 1; day <= lastDay.Day; day++)
            {
                var date = new DateTime(monthStart.Year, monthStart.Month, day);
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || holidays.Any(p => p.Date.Date == date.Date);

                monthModel.Days.Add(new DayModel
                {
                    DayNumber = day,
                    Date = date,
                    IsWeekend = isWeekend
                });
            }

            return monthModel;
        }

        private async Task<DateTime> AddWorkingDays(DateTime start, int workDays)
        {
            DateTime result = start;
            int added = 0;

            while (added < workDays)
            {
                result = result.AddDays(1);
                if (!await IsWeekend(result) && !await IsHoliday(result))
                    added++;
            }

            return result;
        }
        private async Task<bool> IsWeekend(DateTime date)
        {
            return await Task.FromResult(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        private async Task<bool> IsHoliday(DateTime date)
        {
            var flag = await Task.FromResult(holidays.Any(p => p.Date.Day == date.Day && p.Date.Month == date.Month));
            return flag;
        }
        // Modelos para el calendario
        private class MonthModel
        {
            public string MonthName { get; set; }
            public int Year { get; set; }
            public List<DayModel> Days { get; set; }
        }
        private class DayModel
        {
            public int DayNumber { get; set; }
            public DateTime Date { get; set; }
            public bool IsWeekend { get; set; }
            public bool IsEmpty { get; set; }
            public bool HasOperation { get; set; }
            public string Distribution { get; set; }
            public string OperationText { get; set; }
        }

        private class DistributionState
        {
            public Distribution Distribution { get; set; }
            public int CurrentOperationIndex { get; set; } = 0;
            public bool ObservedThisMonth { get; set; } = false;
        }

    }
}