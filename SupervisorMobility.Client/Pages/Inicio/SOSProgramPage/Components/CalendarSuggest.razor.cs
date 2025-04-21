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

            var today = DateTime.Today;
            int year = StartDay.Year;

            var operationsByDist = Dist_Manager
                .Where(d => d.isSelected)
                .ToDictionary(
                    dist => dist.distribution.Description,
                    dist => dist.distribution.Operations.Count()
                );

            int daysSinceLastOperation = SeparateDays + 1;

            for (int monthOffset = 0; monthOffset < 12; monthOffset++)
            {
                var currentDate = StartDay.AddMonths(monthOffset);
                var monthModel = CreateMonthModel(currentDate);

                foreach (var dayModel in monthModel.Days
                    .Where(d =>
                        !d.IsEmpty &&
                        !d.IsWeekend &&
                        (year > today.Year || d.Date >= today))
                    .OrderBy(d => d.Date))
                {
                    daysSinceLastOperation++;

                    if (daysSinceLastOperation > SeparateDays)
                    {
                        // Verifica si hay alguna distribución aún con operaciones pendientes
                        if (operationsByDist.Any(kvp => kvp.Value > 0))
                        {
                            switch (OptionRandom)
                            {
                                case 0:
                                    AssignContinuousDistribution(operationsByDist, ref daysSinceLastOperation, dayModel);
                                    break;
                                case 1:
                                    AssignRandomDistribution(operationsByDist, ref daysSinceLastOperation, dayModel);
                                    break;
                                case 2:
                                    AssignRandomCompleteDistribution(ref daysSinceLastOperation, dayModel, operationsByDist);
                                    break;
                            }
                        }
                    }
                }

                CalendarMonths.Add(monthModel);
            }
        }


        //version 0 sin respetar las reglas de asignacion
        private void GenerateCalendarType3_()
        {
            var operationsByDist = Dist_Manager.Where(d => d.isSelected)
                .ToDictionary(d => d.distribution.Description, d => d.distribution.Operations.Count());

            var calendarMonths = new List<MonthModel>();
            var currentDate = StartDay;
            var currentYear = StartDay.Year;
            var monthsProcessed = 0;

            var remainingDistributions = new Queue<string>(Distributions);
            int initialOpDia = OpDia;

            // Primera vuelta: 1 distribución por mes
            while (remainingDistributions.Count > 0 && monthsProcessed < 12)
            {
                var currentDist = remainingDistributions.Dequeue();
                var monthModel = CreateMonthModel(currentDate);
                var operationsAssigned = 0;
                int workDayCounter = SeparateDays + 1; // para forzar la primera operación

                foreach (var day in monthModel.Days.Where(d =>
                    !d.IsEmpty &&
                    !d.IsWeekend &&
                    (d.Date >= DateTime.Today || currentYear > DateTime.Today.Year)))
                {
                    if (operationsByDist[currentDist] <= 0) break;

                    workDayCounter++;

                    if (workDayCounter > SeparateDays)
                    {
                        var ops = Math.Min(OpDia, operationsByDist[currentDist]);
                        day.HasOperation = true;
                        day.Distribution = currentDist;
                        day.OperationText = $"Dist {currentDist} x{ops}";
                        operationsByDist[currentDist] -= ops;
                        operationsAssigned += ops;
                        workDayCounter = 0;
                    }
                }

                calendarMonths.Add(monthModel);
                currentDate = currentDate.AddMonths(1);
                monthsProcessed++;
            }

            // Segunda vuelta: llenar huecos en meses ya generados
            while (remainingDistributions.Count > 0)
            {
                foreach (var month in calendarMonths)
                {
                    if (remainingDistributions.Count == 0) break;

                    var currentDist = remainingDistributions.Dequeue();
                    int workDayCounter = SeparateDays + 1;

                    foreach (var day in month.Days
                        .Where(d => !d.IsEmpty && !d.IsWeekend && !d.HasOperation &&
                                    (d.Date >= DateTime.Today || currentYear > DateTime.Today.Year))
                        .OrderBy(d => d.Date))
                    {
                        if (operationsByDist[currentDist] <= 0) break;

                        workDayCounter++;

                        if (workDayCounter > SeparateDays)
                        {
                            var ops = Math.Min(OpDia, operationsByDist[currentDist]);
                            day.HasOperation = true;
                            day.Distribution = currentDist;
                            day.OperationText = $"Dist {currentDist} x{ops}";
                            operationsByDist[currentDist] -= ops;
                            workDayCounter = 0;
                        }
                    }
                }
            }

            // Tercera vuelta: si aún quedan operaciones, incrementar OpDia y reiniciar desde el inicio
            while (operationsByDist.Any(kvp => kvp.Value > 0))
            {
                foreach (var month in calendarMonths)
                {
                    foreach (var day in month.Days
                        .Where(d => !d.IsEmpty && !d.IsWeekend && !d.HasOperation &&
                                    (d.Date >= DateTime.Today || currentYear > DateTime.Today.Year))
                        .OrderBy(d => d.Date))
                    {
                        var nextDist = operationsByDist.FirstOrDefault(kvp => kvp.Value > 0);
                        if (string.IsNullOrEmpty(nextDist.Key)) break;

                        var ops = Math.Min(OpDia, nextDist.Value);
                        day.HasOperation = true;
                        day.Distribution = nextDist.Key;
                        day.OperationText = $"Dist {nextDist.Key} x{ops}";
                        operationsByDist[nextDist.Key] -= ops;
                    }
                }

                // Si sigue habiendo operaciones pendientes y no hay espacio, aumentamos la carga diaria
                if (operationsByDist.Any(kvp => kvp.Value > 0))
                {
                    OpDia++;
                }
            }

            CalendarMonths = calendarMonths;
        }

        private void GenerateCalendarType3()
        {
            var operationsByDist = Dist_Manager
                .Where(d => d.isSelected)
                .ToDictionary(d => d.distribution.Description, d => d.distribution.Operations.Count());

            var remainingDistributions = new Queue<string>(Distributions);
            var calendarMonths = new List<MonthModel>();
            int distribucionesPorMes = 1;
            int year = StartDay.Year;
            DateTime today = DateTime.Today;
            int currentLoop = 0;
            int jobsPerDay = OpDia; // funciona como JobsPorDia
            int separation = SeparateDays;

            // Crear calendario de meses del año
            var baseCalendar = Enumerable.Range(0, 12)
                .Select(i => CreateMonthModel(new DateTime(year, 1, 1).AddMonths(i)))
                .ToList();

            calendarMonths.AddRange(baseCalendar);

            while (remainingDistributions.Count > 0)
            {
                foreach (var month in calendarMonths)
                {
                    int distThisMonth = 0;

                    while (remainingDistributions.Count > 0 && distThisMonth < distribucionesPorMes)
                    {
                        var distKey = remainingDistributions.Dequeue();
                        int opsRemaining = operationsByDist[distKey];
                        int workDayCounter = separation + 1;

                        var validDays = month.Days
                            .Where(d =>
                                !d.IsWeekend &&
                                !d.IsEmpty &&
                                !d.HasOperation &&
                                (year > today.Year || d.Date >= today))
                            .OrderBy(d => d.Date)
                            .ToList();

                        foreach (var day in validDays)
                        {
                            if (opsRemaining <= 0) break;

                            workDayCounter++;

                            // Verificamos si el día ya tiene tareas, y si se llegó al máximo permitido
                            int countToday = month.Days.Count(d =>
                                d.HasOperation &&
                                d.Date == day.Date &&
                                d.Distribution == distKey);

                            if (workDayCounter > separation && countToday < jobsPerDay)
                            {
                                int opsToAssign = Math.Min(jobsPerDay, opsRemaining);
                                day.HasOperation = true;
                                day.Distribution = distKey;
                                day.OperationText = $"Dist {distKey} x{opsToAssign}";
                                operationsByDist[distKey] -= opsToAssign;
                                opsRemaining -= opsToAssign;
                                workDayCounter = 0;
                            }
                        }

                        // Si aún quedan operaciones, lo volvemos a poner en la cola
                        if (operationsByDist[distKey] > 0)
                            remainingDistributions.Enqueue(distKey);

                        distThisMonth++;
                    }
                }

                // Si todavía quedan distribuciones siguiente vuelta al año
                if (remainingDistributions.Count > 0)
                {
                    currentLoop++;
                    distribucionesPorMes += 1;

                    // Si todos los días posibles están llenos con jobsPerDay, incrementamos
                    if (AllCalendarFull(calendarMonths, jobsPerDay, year, today))
                    {
                        jobsPerDay++;
                        currentLoop = 0;
                    }
                }
            }

            CalendarMonths = calendarMonths;
        }

        private bool AllCalendarFull(List<MonthModel> months, int maxJobsPerDay, int year, DateTime today)
        {
            foreach (var month in months)
            {
                foreach (var day in month.Days.Where(d =>
                    !d.IsWeekend &&
                    !d.IsEmpty &&
                    (year > today.Year || d.Date >= today)))
                {
                    int opsThatDay = month.Days.Count(d =>
                        d.HasOperation && d.Date == day.Date);

                    if (opsThatDay < maxJobsPerDay)
                        return false;
                }
            }

            return true;
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