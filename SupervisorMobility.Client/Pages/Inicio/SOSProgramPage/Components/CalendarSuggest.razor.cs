using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;
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
        bool Dev_env { get; set; }
        protected override void OnInitialized()
        {
            Dev_env = Environment.IsDevelopment();
            DistributionsInput = string.Join(",", Dist_Manager.Where(d => d.isSelected == true).Select(d => d.distribution.Description).ToList());
            Distributions = Dist_Manager.Where(d => d.isSelected).Select(d => d.distribution.Description).ToList();
            Year = StartDay.Year;
            GenerateRandomColors();
            GenerateCalendar();
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
        // Preparar las operaciones por distribución
        //var operationsByDist = Dist_Manager.Where(d => d.isSelected == true).ToDictionary(
        //    dist => dist.distribution.Description,
        //    dist => dist.distribution.Operations.Count()
        //);

        private void GenerateCalendar()
        {
            CalendarMonths.Clear();

            if (OptionRandom == 3)
            {
                GenerateCalendarType3();
            }
            else
            {
                GenerateCalendarStandardTypes();
            }
        }

        private void GenerateCalendarStandardTypes()
        {
            currentRandomDistribution = null;
            // Preparar las operaciones por distribución
            var operationsByDist = Dist_Manager.Where(d => d.isSelected == true).ToDictionary(
                dist => dist.distribution.Description,
                dist => dist.distribution.Operations.Count()
            );

            // Generar 12 meses
            for (int monthOffset = 0; monthOffset < 12; monthOffset++)
            {
                var currentDate = StartDay.AddMonths(monthOffset);
                var monthModel = CreateMonthModel(currentDate);

                // Contadores para lógica de asignación
                int daysSinceLastOperation = SeparateDays + 1;

                // Generar días del mes
                for (int day = 1; day <= monthModel.Days.Count(d => !d.IsEmpty); day++)
                {
                    var dayModel = monthModel.Days[monthModel.Days.FindIndex(d => d.DayNumber == day)];

                    if (!dayModel.IsWeekend)
                    {
                        daysSinceLastOperation++;

                        if (daysSinceLastOperation > SeparateDays)
                        {
                            switch (OptionRandom)
                            {
                                case 0: AssignContinuousDistribution(operationsByDist, ref daysSinceLastOperation, dayModel); break;
                                case 1: AssignRandomDistribution(operationsByDist, ref daysSinceLastOperation, dayModel); break;
                                case 2: AssignRandomCompleteDistribution(ref daysSinceLastOperation, dayModel, operationsByDist); break;
                            }
                        }
                    }
                }

                CalendarMonths.Add(monthModel);
            }
        }

        private void GenerateCalendarType3()
        {
            var operationsByDist = Dist_Manager.Where(d => d.isSelected == true).ToDictionary(
                dist => dist.distribution.Description,
                dist => dist.distribution.Operations.Count()
            );

            var calendarMonths = new List<MonthModel>();
            var currentDate = StartDay;
            var remainingDistributions = new Queue<string>(Distributions);
            var currentYear = StartDay.Year;
            var monthsProcessed = 0;

            // Primera vuelta: 1 distribución por mes
            while (remainingDistributions.Count > 0 && monthsProcessed < 12)
            {
                var currentDist = remainingDistributions.Dequeue();
                var monthModel = CreateMonthModel(currentDate);
                var operationsAssigned = 0;
                var daysSinceLastOperation = SeparateDays + 1; // Forzar primera operación

                // Procesar todos los días hábiles del mes
                foreach (var dayModel in monthModel.Days.Where(d => !d.IsEmpty && !d.IsWeekend))
                {
                    if (operationsByDist[currentDist] <= 0) break;

                    daysSinceLastOperation++;

                    if (daysSinceLastOperation > SeparateDays)
                    {
                        var ops = Math.Min(OpDia, operationsByDist[currentDist]);
                        dayModel.HasOperation = true;
                        dayModel.Distribution = currentDist;
                        dayModel.OperationText = $"Dist {currentDist} x{ops}";
                        operationsByDist[currentDist] -= ops;
                        operationsAssigned += ops;
                        daysSinceLastOperation = 0;
                    }
                }

                calendarMonths.Add(monthModel);
                currentDate = currentDate.AddMonths(1);
                monthsProcessed++;
            }

            // Segunda vuelta: distribuciones restantes en meses existentes
            if (remainingDistributions.Count > 0)
            {
                foreach (var monthModel in calendarMonths)
                {
                    if (remainingDistributions.Count == 0) break;

                    var currentDist = remainingDistributions.Dequeue();
                    var daysSinceLastOperation = SeparateDays + 1;

                    // Procesar días disponibles (sin operaciones asignadas)
                    foreach (var dayModel in monthModel.Days
                        .Where(d => !d.IsEmpty && !d.IsWeekend && !d.HasOperation)
                        .OrderBy(d => d.Date))
                    {
                        if (operationsByDist[currentDist] <= 0) break;

                        daysSinceLastOperation++;

                        if (daysSinceLastOperation > SeparateDays)
                        {
                            var ops = Math.Min(OpDia, operationsByDist[currentDist]);
                            dayModel.HasOperation = true;
                            dayModel.Distribution = currentDist;
                            dayModel.OperationText = $"Dist {currentDist} x{ops}";
                            operationsByDist[currentDist] -= ops;
                            daysSinceLastOperation = 0;
                        }
                    }
                }
            }

            // Tercera vuelta: si aún quedan operaciones, distribuirlas en espacios vacíos
            if (operationsByDist.Any(kvp => kvp.Value > 0))
            {
                foreach (var monthModel in calendarMonths)
                {
                    foreach (var dayModel in monthModel.Days
                        .Where(d => !d.IsEmpty && !d.IsWeekend && !d.HasOperation)
                        .OrderBy(d => d.Date))
                    {
                        var remainingDist = operationsByDist.FirstOrDefault(kvp => kvp.Value > 0).Key;
                        if (string.IsNullOrEmpty(remainingDist)) break;

                        var ops = Math.Min(OpDia, operationsByDist[remainingDist]);
                        dayModel.HasOperation = true;
                        dayModel.Distribution = remainingDist;
                        dayModel.OperationText = $"Dist {remainingDist} x{ops}";
                        operationsByDist[remainingDist] -= ops;
                    }
                }
            }

            CalendarMonths = calendarMonths;
        }
        
        private DateTime GetNextBusinessDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            }
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);

            return date;
        }

        private MonthModel CreateMonthModel(DateTime currentDate)
        {
            var monthModel = new MonthModel
            {
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentDate.Month),
                Year = currentDate.Year,
                Days = new List<DayModel>()
            };

            var firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Días vacíos al inicio
            int firstDayOfWeek = (int)firstDay.DayOfWeek;
            for (int i = 0; i < firstDayOfWeek; i++)
            {
                monthModel.Days.Add(new DayModel { IsEmpty = true });
            }

            // Días del mes
            for (int day = 1; day <= lastDay.Day; day++)
            {
                var date = new DateTime(currentDate.Year, currentDate.Month, day);
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                monthModel.Days.Add(new DayModel
                {
                    DayNumber = day,
                    Date = date,
                    IsWeekend = isWeekend
                });
            }

            return monthModel;
        }

        private void AssignContinuousDistribution(Dictionary<string, int> operationsByDist, ref int daysSinceLastOperation, DayModel dayModel)
        {
            var nextDist = operationsByDist.FirstOrDefault(kvp => kvp.Value > 0);
            if (!string.IsNullOrEmpty(nextDist.Key))
            {
                var ops = Math.Min(OpDia, nextDist.Value);
                dayModel.HasOperation = true;
                dayModel.Distribution = nextDist.Key;
                dayModel.OperationText = $"Dist {nextDist.Key} x{ops}";
                operationsByDist[nextDist.Key] -= ops;
                daysSinceLastOperation = 0;
            }
        }

        private void AssignRandomDistribution(Dictionary<string, int> operationsByDist, ref int daysSinceLastOperation, DayModel dayModel)
        {
            var availableDists = operationsByDist.Where(kvp => kvp.Value > 0).ToList();
            if (availableDists.Any())
            {
                var randomDist = availableDists[new Random().Next(availableDists.Count)];
                var ops = Math.Min(OpDia, randomDist.Value);
                dayModel.HasOperation = true;
                dayModel.Distribution = randomDist.Key;
                dayModel.OperationText = $"Dist {randomDist.Key} x{ops}";
                operationsByDist[randomDist.Key] -= ops;
                daysSinceLastOperation = 0;
            }
        }

        private void AssignRandomCompleteDistribution(ref int daysSinceLastOperation, DayModel dayModel, Dictionary<string, int> operationsByDist)
        {
            if (currentRandomDistribution == null || operationsByDist[currentRandomDistribution] <= 0)
            {
                currentRandomDistribution = operationsByDist
                    .Where(kvp => kvp.Value > 0)
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefault().Key;
            }

            if (currentRandomDistribution != null && operationsByDist[currentRandomDistribution] > 0)
            {
                var ops = Math.Min(OpDia, operationsByDist[currentRandomDistribution]);
                dayModel.HasOperation = true;
                dayModel.Distribution = currentRandomDistribution;
                dayModel.OperationText = $"Dist {currentRandomDistribution} x{ops}";
                operationsByDist[currentRandomDistribution] -= ops;
                daysSinceLastOperation = 0;
            }
        }

        private int GenerateRandomOperations(int dist)
        {
            return new Random().Next(3, 11) * OpDia; // Multiplicar por OpDia para tener suficiente
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
    }
}