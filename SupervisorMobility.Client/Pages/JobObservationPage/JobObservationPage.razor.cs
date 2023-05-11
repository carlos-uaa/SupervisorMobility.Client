using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System;
using System.Runtime.InteropServices;

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

        public Color color = Color.Default;
        public bool filters = false;
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();

        public bool operationFlag = false;
        public int distributionId;
        public int operationId;
        public DateTime? filterDate = null;
        public int operatorId;
        public int statusId;
        public int idFilter;

        //Filters
        public List<JobObservation> _filterJobObservation { get; set; } = new();
        public List<JobObservation> _filterPlannedJobObservation { get; set; } = new();
        public List<JobObservation> _filterInProgressJobObservation { get; set; } = new();
        public List<JobObservation> _filterLateJobObservation { get; set; } = new();
        public List<JobObservation> _filterUnderReviewJobObservation { get; set; } = new();
        public List<JobObservation> _filterRejectedJobObservation { get; set; } = new();
        public List<JobObservation> _filterFinishedJobObservation { get; set; } = new();


        //Job observations status lists.
        public List<JobObservation> _jobObservation { get; set; } = new();
        public List<JobObservation> _plannedJobObservation { get; set; } = new();
        public List<JobObservation> _inProgressJobObservation { get; set; } = new();
        public List<JobObservation> _lateJobObservation { get; set; } = new();
        public List<JobObservation> _underReviewJobObservation { get; set; } = new();
        public List<JobObservation> _rejectedJobObservation { get; set; } = new();
        public List<JobObservation> _finishedJobObservation { get; set; } = new();



        public string totalPlanned;
        public string totalInProgress;
        public string totalLate;
        public string totalUnderReview;
        public string totalRejected;
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

                await GetUserAsync();
                await LateDates();

                _jobObservation = await JobObservationService.GetAllJobObservations();
                _filterJobObservation = _jobObservation;
                foreach (var jobobs in _jobObservation)
                {
                    switch (jobobs.Status)
                    {
                        case 1: _plannedJobObservation.Add(jobobs); break;
                        case 2: _inProgressJobObservation.Add(jobobs); break;
                        case 3: _lateJobObservation.Add(jobobs); break;
                        case 4: _underReviewJobObservation.Add(jobobs); break;
                        case 5: _rejectedJobObservation.Add(jobobs); break;
                        case 6: _finishedJobObservation.Add(jobobs); break;
                    }
                }

                _filterPlannedJobObservation = _plannedJobObservation;
                _filterInProgressJobObservation = _inProgressJobObservation;
                _filterLateJobObservation= _lateJobObservation;
                _filterUnderReviewJobObservation = _underReviewJobObservation;
                _filterRejectedJobObservation = _rejectedJobObservation;
                _filterFinishedJobObservation= _finishedJobObservation;


                totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
                totalInProgress = "In Progress (" + _inProgressJobObservation.Count + ")";
                totalLate = "Late (" + _lateJobObservation.Count +")";
                totalUnderReview = "Under Review (" + _underReviewJobObservation.Count + ")";
                totalRejected = "Rejected (" + _rejectedJobObservation.Count + ")";
                totalFinished = "Finished (" + _finishedJobObservation.Count +")";


                if(user != null)
                {
                    _distributions = await DistributionService.GetDistributionsWithCollections((int)user.PlantId, (int)user.AreaId);

                }

                //operator User
                users = await UsersService.GetUsers();
                foreach (var operatorUser in users)
                {
                    if (user != null && operatorUser.AreaId == user.AreaId && operatorUser.UserType == 4)
                    {
                        operatorUsers.Add(operatorUser);
                    }
                }

            }
        }

        public void ActiveFilters()
        {
            filters = !filters;

            _jobObservation = _filterJobObservation;
            _plannedJobObservation= _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation= _filterUnderReviewJobObservation;
            _rejectedJobObservation= _filterRejectedJobObservation;
            _finishedJobObservation= _filterFinishedJobObservation;

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();

            if(color == Color.Info)
            {
                color = Color.Default;
            }
            else
            {
                color = Color.Info;
            }

        }


        public void ClearFilters()
        {
            _jobObservation = _filterJobObservation;
            _plannedJobObservation = _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation = _filterUnderReviewJobObservation;
            _rejectedJobObservation = _filterRejectedJobObservation;
            _finishedJobObservation = _filterFinishedJobObservation;


            totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
            totalInProgress = "In Progress (" + _inProgressJobObservation.Count + ")";
            totalLate = "Late (" + _lateJobObservation.Count + ")";
            totalUnderReview = "Under Review (" + _underReviewJobObservation.Count + ")";
            totalRejected = "Rejected (" + _rejectedJobObservation.Count + ")";
            totalFinished = "Finished (" + _finishedJobObservation.Count + ")";

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();
        }

        //Filters
        private void FilterDistributions()
        {
            operationId = new();
            _jobObservation = _filterJobObservation;
            _plannedJobObservation = _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation = _filterUnderReviewJobObservation;
            _rejectedJobObservation = _filterRejectedJobObservation;
            _finishedJobObservation = _filterFinishedJobObservation;

            operationFlag = true;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == distributionId)].Operations;
            _jobObservation = _jobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _finishedJobObservation = _filterFinishedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();


            if (statusId != default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
            }
            if (operatorId != default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
            }
            if (filterDate != null)
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
            }
            if (idFilter != default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
            }

            totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
            totalInProgress = "In Progress (" + _inProgressJobObservation.Count + ")";
            totalLate = "Late (" + _lateJobObservation.Count + ")";
            totalUnderReview = "Under Review (" + _underReviewJobObservation.Count + ")";
            totalRejected = "Rejected (" + _rejectedJobObservation.Count + ")";
            totalFinished = "Finished (" + _finishedJobObservation.Count + ")";
        }

        private void Filters()
        {
            _jobObservation = _filterJobObservation;
            _plannedJobObservation = _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation = _filterUnderReviewJobObservation;
            _rejectedJobObservation = _filterRejectedJobObservation;
            _finishedJobObservation = _filterFinishedJobObservation;

            if (distributionId != default(int)) {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            }
            if (operationId!= default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
            }
            if (statusId != default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
            }
            if (operatorId != default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
            }
            if(filterDate != null)
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
            }
            if(idFilter != default(int))
            {
                _jobObservation = _jobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
            }

            totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
            totalInProgress = "In Progress (" + _inProgressJobObservation.Count + ")";
            totalLate = "Late (" + _lateJobObservation.Count + ")";
            totalUnderReview = "Under Review (" + _underReviewJobObservation.Count + ")";
            totalRejected = "Rejected (" + _rejectedJobObservation.Count + ")";
            totalFinished = "Finished (" + _finishedJobObservation.Count + ")";
        }

        public void ClearStatus()
        {
            statusId = new();
        }

        //Local storage user
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


        public async Task LateDates()
        {
            _jobObservation = await JobObservationService.GetAllJobObservations();
            foreach(var jobobs in _jobObservation)
            {
                if(Convert.ToDateTime(jobobs.EndDate?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 6 && jobobs.Status != 3)
                {
                    jobobs.Status = 3;

                    await JobObservationService.UpdateJobObservation(jobobs, user.Email);
                }
            }
        }


        async Task DeleteJobObservation(int jobObservationId)
        {
            _jobObservation.RemoveAll(jobObservation => jobObservation.JobObservationId == jobObservationId);
            await JobObservationService.DeleteJobObservation(jobObservationId);
            _plannedJobObservation.Clear();
            _inProgressJobObservation.Clear();
            _lateJobObservation.Clear();
            _underReviewJobObservation.Clear();
            _rejectedJobObservation.Clear();
            _finishedJobObservation.Clear();

            foreach (var jobobs in _jobObservation)
            {
                switch (jobobs.Status)
                {
                    case 1: _plannedJobObservation.Add(jobobs); break;
                    case 2: _inProgressJobObservation.Add(jobobs); break;
                    case 3: _lateJobObservation.Add(jobobs); break;
                    case 4: _underReviewJobObservation.Add(jobobs); break;
                    case 5: _rejectedJobObservation.Add(jobobs); break;
                    case 6: _finishedJobObservation.Add(jobobs); break;
                }
            }


            totalPlanned = "Planned (" + _plannedJobObservation.Count + ")";
            totalInProgress = "In Progress (" + _inProgressJobObservation.Count + ")";
            totalLate = "Late (" + _lateJobObservation.Count + ")";
            totalUnderReview = "Under Review (" + _underReviewJobObservation.Count + ")";
            totalRejected = "Rejected (" + _rejectedJobObservation.Count + ")";
            totalFinished = "Finished (" + _finishedJobObservation.Count + ")";

            visibleDelete = false;

            StateHasChanged();
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
            if (element.StartDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operator.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.JobObservationId} {element.Supervisor.Name} {element.Operator}".Contains(searchString))
                return true;
            return false;
        }

        public void ShowError()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Select a Distribution first", Severity.Error);
        }

        //Delete Job observation
        private bool visibleDelete = false;
        public int deleteJobObservationId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteJobObservationId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter };

    }
}
