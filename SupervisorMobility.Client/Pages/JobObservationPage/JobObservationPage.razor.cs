using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class JobObservationPage
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation", disabled: true),
        };

        //Objects
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";

        //Filters
        public Color color = Color.Default;
        public bool filters = false;
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();

        public int distributionId;
        public int operationId;
        public DateTime? filterDate = new();
        public int OperatorId;
        public int StatusId;
        // Objects
        public List<JobObservation> _jobObservation { get; set; } = new();
        public List<JobObservation> _plannedJobObservation { get; set; } = new();
        public List<JobObservation> _inProgressjobObservation { get; set; } = new();
        public List<JobObservation> _latejobObservation { get; set; } = new();
        public List<JobObservation> _finishedjobObservation { get; set; } = new();

        public string totalPlanned;
        public string totalInProgress;
        public string totalLate;
        public string totalFinished;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();

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
                await LateDates();


                _jobObservation = await JobObservationService.GetAllJobObservations();

                foreach (var jobobs in _jobObservation)
                {
                    switch (jobobs.Status)
                    {
                        case 1: _plannedJobObservation.Add(jobobs); break;
                        case 2: _inProgressjobObservation.Add(jobobs); break;
                        case 3: _latejobObservation.Add(jobobs); break;
                        case 4: _finishedjobObservation.Add(jobobs); break;
                    }
                }

                totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
                totalInProgress = "In Progress (" + _inProgressjobObservation.Count + ")";
                totalLate = "Late (" + _latejobObservation.Count +")";
                totalFinished = "Finished (" + _finishedjobObservation.Count +")";

                await GetUserAsync();

                if(user != null)
                {
                    _distributions = await DistributionService.GetDistributionsWithCollections(user.PlantId, user.AreaId);

                }

                //operator User
                users = await UsersService.GetUsers();
                foreach (var operatorUser in users)
                {
                    if (user != null && operatorUser.AreaId == user.AreaId && operatorUser.IsOperator)
                    {
                        operatorUsers.Add(operatorUser);
                    }
                }

            }
        }

        public void ActiveFilters()
        {
            filters = !filters;
            if(color == Color.Info)
            {
                color = Color.Default;
            }
            else
            {
                color = Color.Info;
            }

        }

        //Filters
        private async Task FilterDistributions()
        {

        }

        private async Task FilterOperations()
        {

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


        public async Task LateDates()
        {
            _jobObservation = await JobObservationService.GetAllJobObservations();
            foreach(var jobobs in _jobObservation)
            {
                if(Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 4)
                {
                    jobobs.Status = 3;
                    await JobObservationService.UpdateJobObservation(jobobs);
                }
            }
        }


        async Task DeleteJobObservation(int jobObservationId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this job observation?");

            if (confirm)
            {
                _jobObservation.RemoveAll(jobObservation => jobObservation.JobObservationId == jobObservationId);
                await JobObservationService.DeleteJobObservation(jobObservationId);
                _plannedJobObservation.Clear();
                _inProgressjobObservation.Clear();
                _latejobObservation.Clear();
                _finishedjobObservation.Clear();

                foreach (var jobobs in _jobObservation)
                {
                    switch (jobobs.Status)
                    {
                        case 1: _plannedJobObservation.Add(jobobs); break;
                        case 2: _inProgressjobObservation.Add(jobobs); break;
                        case 3: _latejobObservation.Add(jobobs); break;
                        case 4: _finishedjobObservation.Add(jobobs); break;
                    }
                }

                totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
                totalInProgress = "In Progress (" + _inProgressjobObservation.Count + ")";
                totalLate = "Late (" + _latejobObservation.Count + ")";
                totalFinished = "Finished (" + _finishedjobObservation.Count + ")";

                StateHasChanged();
            }
        }

        void EditJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        void CreateJobObservation()
        {
            var date = DateTime.Now.ToShortDateString();
            date = date.Replace("/", "-");
            NavigationManager.NavigateTo($"jobobservation/createjobobservation/{date}");
        }

        void PlanJobObservation()
        {
            var date = DateTime.Now.ToShortDateString();
            date = date.Replace("/", "-");
            NavigationManager.NavigateTo($"jobobservation/planjobobservation/{date}");
        }

        public bool flagJob = false;
        private bool visible = false;
        private int jobId;
        private void OpenDialog2(int id)
        {
            jobId = id;
            visible = true;
        }
        void Close() => visible = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private bool FilterFunc(JobObservation element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.JobObservationId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Distribution.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operation.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.DateStart.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operator.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.JobObservationId} {element.Supervisor.Name} {element.Operator}".Contains(searchString))
                return true;
            return false;
        }


    }
}
