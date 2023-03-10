using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System;
using System.Globalization;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class CreateJobObservation
    {

        [Parameter]
        public string date { get; set; }
        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? startHour = new TimeSpan(12, 00, 00);
        TimeSpan? endHour = new TimeSpan(13, 00, 00);
        DateTime newDate1;
        DateTime newDate2;

        List<JobObservation> _jobObservations;
        List<Plant> _plants { get; set; } = new();
        List<Product> _products { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Lup> _tempLup { get; set; } = new();
        Lup lup { get; set; } = new();
        List<Lup> _lup { get; set; } = new();

        public JobObservation _jobObservation { get; set; } = new();

        int[] models = new int[5];
        string[] cicles = new string[5] { "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00" };

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        //timer
        const string DEFAULT_TIME = "00:00:00";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;
        public int opt = 1;

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("New Job Observation", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {
            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);


            foreach (var kvp in _glosaryInfo)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Description);
            }
            date = date.Replace("-", "/");

            _jobObservation.IsActive= true;
            _jobObservation.DateStart = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.DateEnd = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.Option = 3;
            _plants = await PlantServices.GetPlants();
            _products = await ProductService.GetProducts();
        }
        private async void ShowAreas()
        {
            _jobObservation.AreaId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private void ShowOperations()
        {
            _jobObservation.OperationId = 0;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
        }

        private async Task CreateNewJobObservation()
        {
            Console.WriteLine(startHour);
            hour1 = _jobObservation.DateStart?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.DateEnd?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
                Console.WriteLine("Unable to parse '{0}'", hour1);


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
                Console.WriteLine("Unable to parse '{0}'", hour2);
 
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles= cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];
            _jobObservation.DateStart = newDate1;
            _jobObservation.DateEnd = newDate2;
            _jobObservation.Status = 1;

            var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                if(_tempLup.Count > 0)
                {
                    _jobObservations = await JobObservationService.GetAllJobObservations();
                    foreach(var temp in _tempLup)
                    {
                        temp.JobObservationId = _jobObservations.Last().JobObservationId;
                        var result2 = await LupService.CreateLup(temp);
                        if (result2 != null)
                        {
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Job observation Lup item Created", Severity.Info);
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Error in Lup", Severity.Error);
                        }
                    }
                }

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

        }

        void CancelCreateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }

        //timer
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            switch (opt)
            {
                case 1:
                    cicles[0] = elapsedTime; break;
                case 2:
                    cicles[1] = elapsedTime; break;
                case 3:
                    cicles[2] = elapsedTime; break;
                case 4:
                    cicles[3] = elapsedTime; break;
                case 5:
                    cicles[4] = elapsedTime; break;
            }
            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0,12);
            StateHasChanged();
        }

        void StartTimer()
        {
            startTime = DateTime.Now;
            timer = new System.Timers.Timer(1);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            isRunning = true;
        }

        void StopTimer()
        {
            isRunning = false;
            Console.WriteLine($"Elapsed Time: {elapsedTime}");
            timer.Enabled = false;
            elapsedTime = DEFAULT_TIME;
        }

        void OnTimerChanged()
        {
            if (!isRunning)
                StartTimer();
            else
                StopTimer();
        }

        void Option1() => opt = 1;
        void Option2() => opt = 2;
        void Option3() => opt = 3;
        void Option4() => opt = 4;
        void Option5() => opt = 5;


        //Lup
        void Closed(MudChip chip)
        {
            // react to chip closed
        }
        public void AddTempLup(int pillar)
        {

            switch (pillar)
            {
                case 1:
                    if (_jobObservation.SArea != null && _jobObservation.SArea.Length > 0)
                    {
                        lup.Oportunity = _jobObservation.SArea;
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error S Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 2:
                    if (_jobObservation.QArea != null && _jobObservation.QArea.Length > 0)
                    {
                        lup.Oportunity = _jobObservation.QArea;
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error Q Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 3:
                    if (_jobObservation.DArea != null && _jobObservation.DArea.Length > 0)
                    {
                        lup.Oportunity = _jobObservation.DArea;
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error D Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 4:
                    if (_jobObservation.CArea != null && _jobObservation.CArea.Length > 0)
                    {
                        lup.Oportunity = _jobObservation.CArea;
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error C Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 5:
                    if (_jobObservation.OthersArea != null && _jobObservation.OthersArea.Length > 0)
                    {
                        lup.Oportunity = _jobObservation.OthersArea;
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error Others Area is empty", Severity.Error);
                        return;
                    }
                    break;
            }

            lup.Observer = _jobObservation.Observer;
            lup.JobObservationId = 0;
            lup.Pillar = pillar;
            lup.Status = 1;
            lup.CreatedDate = DateTime.Now;
            lup.IsActive = true;

            _tempLup.Add(lup);
            lup = new();
            
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Lup item added", Severity.Info);



        }

        public void DeleteLup(Lup lup)
        {
            switch (lup.Pillar) {
                case 1: _jobObservation.SArea = ""; break;
                case 2: _jobObservation.QArea = ""; break;
                case 3: _jobObservation.DArea = ""; break;
                case 4: _jobObservation.CArea = ""; break;
                case 5: _jobObservation.OthersArea = ""; break; 
            }
            _tempLup.Remove(lup);
        }

    }
}
