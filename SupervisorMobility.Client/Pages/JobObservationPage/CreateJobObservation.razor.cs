using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
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

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

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

        //Past Job observation
        //Lup Modal
        private bool visiblePast = false;
        private bool visibleLup = false;
        private int lupId;

        private DialogOptions dialogLup = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Past job observation
        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();



        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("New Job Observation", href: "", disabled: true)
        };


        //User
        private string json = string.Empty;
        public User user = new();

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

            await GetUserAsync();

            if (user != null)
            {
                _plants = await PlantServices.GetPlants();
                _jobObservation.PlantId = user.PlantId;

                _jobObservation.AreaId = user.AreaId;

                _areas = await AreaServices.GetAreas(user.PlantId);
                _jobObservation.Observer = user.Name;
                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                StateHasChanged();
            }

            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.Observer == user.Name && Convert.ToDateTime(job.DateStart?.ToShortDateString()).Date <= Convert.ToDateTime(_jobObservation.DateStart?.ToShortDateString()).Date)
                    {

                        pastjobObservations.Add(job);

                        pastJob = await JobObservationService.GetJobObservationWithLup(job.JobObservationId);
                        foreach (var lups in pastJob.Lup)
                        {
                            pastLup.Add(lups);
                        }
                    }

                }

            }
            pastjobObservations = pastjobObservations.OrderBy(x => x.DateStart).ToList();

        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
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
                    if (areaS != null && areaS.Length > 0)
                    {
                        lup.Oportunity = areaS;
                        areaS = "";
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
                    if (areaQ != null && areaQ.Length > 0)
                    {
                        lup.Oportunity = areaQ;
                        areaQ = "";
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
                    if (areaD != null && areaD.Length > 0)
                    {
                        lup.Oportunity = areaD;
                        areaD = "";
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
                    if (areaC != null && areaC.Length > 0)
                    {
                        lup.Oportunity = areaC;
                        areaC = "";
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
                    if (areaOther != null && areaOther.Length > 0)
                    {
                        lup.Oportunity = areaOther;
                        areaOther = "";
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
                case 1: areaS = ""; break;
                case 2: areaQ = ""; break;
                case 3: areaD = ""; break;
                case 4: areaC = ""; break;
                case 5: areaOther = ""; break; 
            }
            _tempLup.Remove(lup);
        }

        //Past Job observation
        private void OpenDialogLup(int id)
        {
            lupId = id;
            visibleLup = true;
        }

        private void OpenDialogPastJobObservations()
        {
            visiblePast = true;
        }

        void CloseLup() => visibleLup = false;
        void CloseOverdue() => visiblePast = false;

        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }
    }
}
