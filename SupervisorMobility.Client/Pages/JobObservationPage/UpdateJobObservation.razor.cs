using Blazorise.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Security;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }

        public JobObservation _jobObservation { get; set; } = new();
        public Lup lup { get; set; } = new();
        public JobObservation _lupJobObservations { get; set; } = new();

        public string hour1 { get; set; }
        public string hour2 { get; set; }

        DateTime newDate1;
        DateTime newDate2;

        //Objects
        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Product> _products { get; set; } = new();
        private AssyChart _assychart { get; set; } = new AssyChart();
        private string messageErrorFolders;
        private bool searchAssychart = false;


        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cycles = new string[5];

        //timer
        const string DEFAULT_TIME = "00:00:00.000";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;
        public int opt = 1;

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;


        //Lup Modal
        private bool visibleLup = false;
        private bool visiblePast = false;
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
            new BreadcrumbItem("Update Job Observation", href: "", disabled: true)
        };

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();

        public string route = string.Empty;
        public bool sign = false;
        void Closed(MudChip chip)
        {
            // react to chip closed
        }

        //Change date
        DateTime? plannedStartDate = new();
        DateTime? plannedEndDate = new();

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected async override Task OnInitializedAsync()
        {
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
  
                _jobObservation.Supervisor = new();
                //glosary
                glosary = await GlosaryService.GetGlosary();
                _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

                _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);

                _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);

                //Change date

                startHour = _jobObservation.StartDate?.TimeOfDay;
                endHour = _jobObservation.EndDate?.TimeOfDay;


                if(_jobObservation.PlannedStartDate != null)
                {
                    plannedStartDate = _jobObservation.PlannedStartDate;
                    plannedEndDate = _jobObservation.PlannedEndDate;
                }
                else
                {                
                    plannedStartDate = _jobObservation.StartDate;
                    plannedEndDate = _jobObservation.EndDate;
                }

                _plants = await PlantServices.GetPlants();
                //_products = await ProductService.GetProducts();
                _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

                _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;

                var prod = _jobObservation.Models.Split('|');
                models[0] = Int32.Parse(prod[0]);
                models[1] = Int32.Parse(prod[1]);
                models[2] = Int32.Parse(prod[2]);
                models[3] = Int32.Parse(prod[3]);
                models[4] = Int32.Parse(prod[4]);
                cycles = _jobObservation.Cicles.Split('|');

                users = await UsersService.GetUsers();
                foreach (var operatorUser in users)
                {
                    if (user != null && operatorUser.AreaId == operatorUser.AreaId && operatorUser.UserType == 4)
                    {
                        operatorUsers.Add(operatorUser);
                    }
                }


                await GetUserAsync();

                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                            && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
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
                pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();


                if (_jobObservation.PlantId != 0)
                {
                    if (_jobObservation.AreaId != 0)
                    {
                        if (_jobObservation.DistributionId != 0)
                        {
                            if (_jobObservation.DistributionId != 0)
                            {
                                try
                                {
                                    _assychart = await AssychartServices.GetAssyChartAdvance(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, _jobObservation.OperationId);
                                    if (_assychart == null)
                                        messageErrorFolders = "The folders with the information provided were not located.";
                                    else
                                        searchAssychart = true;
                                }
                                catch (Exception ex)
                                {
                                    messageErrorFolders = "The folders with the information provided were not located.";
                                }

                            }
                            else
                            {
                                messageErrorFolders = "Job Observation does not contain a valid operation";
                                Console.WriteLine("missing plant");
                            }
                        }
                        else
                        {
                            messageErrorFolders = "Job Observation does not contain a valid distribution";
                            Console.WriteLine("missing plant");
                        }
                    }
                    else
                    {
                        messageErrorFolders = "Job Observation does not contain a valid area";
                        Console.WriteLine("missing plant");
                    }
                }
                else
                {
                    messageErrorFolders = "Job Observation does not contain a valid plant";
                    Console.WriteLine("missing plant");
                }
            }

        }

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

        void history()
        {
            NavigationManager.NavigateTo($"jobobservation/history/{JobObservationId}");
        }



        //Change date modal and function
        private bool visible = false;
        private void OpenCommentDialog()
        {
            visible = true;
        }
        void Close() => visible = false;
        private DialogOptions dialogCommentOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        public async Task ChangeDate()
        {

            if (_jobObservation.Justification == null || _jobObservation.Justification == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add a comment", Severity.Error);
                return;
            }

            hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date Start", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour1);
            }


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date End", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour2);
            }

            _jobObservation.StartDate = newDate1;
            _jobObservation.EndDate = newDate2;

            _jobObservation.PlannedStartDate = newDate1;
            _jobObservation.PlannedEndDate = newDate2;

            Console.WriteLine(_jobObservation.StartDate);
            Console.WriteLine(_jobObservation.EndDate);


            if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to change the date first", Severity.Error);
                return;
            }

            if(_jobObservation.Status == 3)
            {
                _jobObservation.Status = 1;
            }

            var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.Email);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Date Changed in Job Observation {_jobObservation.JobObservationId}", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }


        //In progress
        private async Task SaveProgressJobObservation()
        {
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }

            startHour = DateTime.Now.TimeOfDay;
            
            hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour?.ToString("hh\\:mm\\:ss")}";
            hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date Start", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour1);
            }


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date End", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour2);
            }
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.StartDate = newDate1;
            _jobObservation.EndDate = newDate2;
            _jobObservation.Status = 2;

            if (_jobObservation.Justification == "")
            {
                _jobObservation.Justification = null;
            }

            var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.Email);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Updated", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }


        //Cancel
        void CancelUpdateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }


        //Under Review Job observation
        public async void UnderReviewJobObservation()
        {
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }

            hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date Start", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour1);
            }


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date End", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour2);
            }
            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Operator Signature", Severity.Error);
                return;
            }
            _jobObservation.StartDate = newDate1;
            _jobObservation.EndDate = newDate2;

            _jobObservation.FinishedDate = DateTime.Now;
            Console.WriteLine(_jobObservation.FinishedDate);

            if (_jobObservation.SsvSignature.IsNullOrEmpty())
            {
                _jobObservation.Status = 4;
            }
            else
            {
                _jobObservation.Status = 6;
            }


            var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.Email);


            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Finished", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert

        }


        //Reject Job observation
        async void Reject()
        {

            if (_jobObservation.SsvCommentary == null || _jobObservation.SsvCommentary == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add a commentary to reject the job observation", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }


            hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date Start", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour1);
            }


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date End", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour2);
            }

            _jobObservation.StartDate = newDate1;
            _jobObservation.EndDate = newDate2;

            _jobObservation.Status = 5;
            var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.Email);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Rejected", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }


        //Finished Job observation
        private bool visibleSign = false;
        private void OpenSignComment()
        {
            visibleSign = true;
        }
        void CloseSign() => visibleSign = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

        public async Task SignDate()
        {

            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature doesn't match", Severity.Error);
                return;
            }


            endHour = DateTime.Now.TimeOfDay;

            hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour?.ToString("hh\\:mm\\:ss")}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date Start", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour1);
            }


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Date End", Severity.Error);
                Console.WriteLine("Unable to parse '{0}'", hour2);
            }

            _jobObservation.StartDate = newDate1;
            _jobObservation.EndDate = newDate2;
            _jobObservation.SsvSignature = "Signed";
            _jobObservation.Status = 6;
            var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.Email);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Finished", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }


        //Job observations
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
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private async void ShowOperations()
        {
            _jobObservation.OperationId = 0;
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }
        


        //timer
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            TimeSpan hundreths;
            int centiseconds = 0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = (int)hundreths.TotalMilliseconds / 10;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cycles[0] = centiseconds.ToString(); break;
                case 2:
                    cycles[1] = centiseconds.ToString(); break;
                case 3:
                    cycles[2] = centiseconds.ToString(); break;
                case 4:
                    cycles[3] = centiseconds.ToString(); break;
                case 5:
                    cycles[4] = centiseconds.ToString(); break;
            }
            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0, 12);
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
            TimeSpan hundreths;
            int centiseconds = 0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = (int)hundreths.TotalMilliseconds / 10;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cycles[0] = centiseconds.ToString(); break;
                case 2:
                    cycles[1] = centiseconds.ToString(); break;
                case 3:
                    cycles[2] = centiseconds.ToString(); break;
                case 4:
                    cycles[3] = centiseconds.ToString(); break;
                case 5:
                    cycles[4] = centiseconds.ToString(); break;
            }
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


        //Lup
        public async void AddLup(int pillar)
        {

            switch (pillar)
            {
                case 1: 
                    if(areaS != null && areaS.Length > 0)
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

            lup.Observer = _jobObservation.Supervisor.Name;
            lup.JobObservationId = _jobObservation.JobObservationId;
            lup.Pillar = pillar;
            lup.Status = 1;
            lup.CreatedDate= DateTime.Now;
            lup.IsActive = true;


            var result = await LupService.CreateLup(lup);
            if (result != null)
            {
                _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);

                await GetUserAsync();

                pastjobObservations = new();
                pastLup = new();
                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                            && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
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
                pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();
                StateHasChanged();

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup Created", Severity.Info);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Lup", Severity.Error);
            }
        }


        //Past Job Observations Modal
        private void OpenDialogPastJobObservations()
        {
            visiblePast = true;
        }
        void CloseOverdue() => visiblePast = false;


        //Lup Modal
        private void OpenDialogLup(int id)
        {
            lupId = id;
            visibleLup = true;
        }
  
        void CloseLup() => visibleLup = false;
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        async Task DeleteLup(int lupId)
        {
            await LupService.DeleteLup(lupId);

            _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);

            await GetUserAsync();

            pastjobObservations = new();
            pastLup = new();
            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                        && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
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
            pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();


            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Lup Deleted", Severity.Info);
            visibleDelete = false;

            StateHasChanged();
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        //HOE, CCP, GOS
        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool folderError = false;


        private CDMS_CCP_Archives CcpFilesInFolder = new CDMS_CCP_Archives();
        private CDMS_HOE_Archives HoeFilesInFolder = new CDMS_HOE_Archives();
        private CDMS_GOS_Archives GosFilesInFolder = new CDMS_GOS_Archives();

        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        private async void OpenDialogGOS(string ruta)
        {
            GOSrute = ruta;
            GosDialog = true;
            folderError = false;

            Console.WriteLine($"gos {ruta}");

            GosFilesInFolder = new CDMS_GOS_Archives();

            GosFilesInFolder = await CDMSServices.GetFilesGOS(ruta);
            if (GosFilesInFolder == null)
                folderError = true;


            StateHasChanged();
        }
        void CloseGos() => GosDialog = false;

        private async void OpenDialogCcp(string ruta)
        {
            CCPrute = ruta;
            CcpDialog = true;
            folderError = false;
            Console.WriteLine($"Cpc {ruta}");

            CcpFilesInFolder = new CDMS_CCP_Archives();
            CcpFilesInFolder = await CDMSServices.GetFilesCCP(ruta);
            if (CcpFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseCcp() => CcpDialog = false;

        private async void OpenDialogHoe(string ruta)
        {
            HOErute = ruta;
            HoeDialog = true;
            Console.WriteLine($"hoe {ruta}");

            folderError = false;
            HoeFilesInFolder = new CDMS_HOE_Archives();
            HoeFilesInFolder = await CDMSServices.GetFilesHOE(ruta);
            if (HoeFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseHoe() => HoeDialog = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

        private async Task DownloadFileFromURL_HOE(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }
        private async Task DownloadFileFromURL_CCP(string urlroute, string namefile)
        {

            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkCCP(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempCCP(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }
        }
        private async Task DownloadFileFromURL_GOS(string urlroute, string namefile)
        {
            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkGOS(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempGOS(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");

                }
            }
            }

            public void ShowDateMessage()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"To change the date, press the button Change Date", Severity.Info);
        }

        public void ShowHourMessage()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"To change the hour, press the button Change Date", Severity.Info);
        }


        //Delete lup
        private bool visibleDelete = false;
        public int deleteLupId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteLupId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter };

    }
}
