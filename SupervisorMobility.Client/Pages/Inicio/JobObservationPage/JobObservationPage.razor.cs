using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class JobObservationPage
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

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
        public List<JobObservation> _filterProgrammedJobObservation { get; set; } = new();


        //Job observations status lists.
        public List<JobObservation> _jobObservations { get; set; } = new();
        public List<JobObservation> _jobObservationsAux { get; set; } = new();
        public List<JobObservation> _plannedJobObservation { get; set; } = new();
        public List<JobObservation> _inProgressJobObservation { get; set; } = new();
        public List<JobObservation> _lateJobObservation { get; set; } = new();
        public List<JobObservation> _underReviewJobObservation { get; set; } = new();
        public List<JobObservation> _rejectedJobObservation { get; set; } = new();
        public List<JobObservation> _finishedJobObservation { get; set; } = new();

        public List<JobObservation> _SOSJobobservation { get; set; } = new();

        //Admin
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();

        public string totalPlanned;
        public string totalInProgress;
        public string totalLate;
        public string totalUnderReview;
        public string totalRejected;
        public string totalFinished;
        public string totalProgrammed;

        public int plantId;
        public int areaId;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();



        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation", disabled: true),
            };

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
                _jobObservations.Clear();

                if(user != null)
                {
                    _jobObservationsAux = await JobObservationService.GetAllJobObservations(true,true);

                    if (user.UserType == 1 || user.UserType == 6)
                    {
                        ClearFilters();
                        _plants = await PlantServices.GetPlants();
                        _plants = _plants.OrderBy(p => p.Description).ToList();

                        foreach (var jobobs in _jobObservationsAux)
                        {
                            if(jobobs.Status != 7)
                            {
                                _jobObservations.Add(jobobs);
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
                            else
                            {
                                _SOSJobobservation.Add(jobobs);
                            }
                        }

                    }
                    else if(user.UserType == 2)
                    {
                        ClearFilters();
                        plantId = (int)user.PlantId;

                        if (user.Areas != null)
                        {
                            _areas = user.Areas.ToList();
                            _areas.OrderBy(a => a.Description).ToList();
                        }
                        foreach (var jobobs in _jobObservationsAux)
                        {
                            if (plantId == jobobs.PlantId)
                            {
                                foreach (User usr in user.Subordinates)
                                {
                                    if (jobobs.SupervisorId == usr.UserId && jobobs.PlantId == plantId)
                                    {
                                        if(jobobs.Status != 7)
                                        {
                                            _jobObservations.Add(jobobs);
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
                                        else
                                        {
                                            _SOSJobobservation.Add(jobobs);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    else if(user.UserType == 3)
                    {
                        plantId = (int)user.PlantId;
                        areaId = (int)user.AreaId;

                        foreach (var jobobs in _jobObservationsAux)
                        {
                            if(plantId == jobobs.PlantId &&  areaId == jobobs.AreaId && user.UserId == jobobs.SupervisorId)
                            {
                                if(jobobs.Status != 7)
                                {
                                    _jobObservations.Add(jobobs);
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
                                else
                                {
                                    _SOSJobobservation.Add(jobobs);
                                }
                            }
                        }

                        _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);

                        //operator User
                        //users = await UsersService.GetUsers(false, false);
                        //foreach (var operatorUser in users)
                        //{
                        //    if (operatorUser.PlantId == plantId && operatorUser.AreaId == areaId && operatorUser.UserType == 4)
                        //    {
                        //        operatorUsers.Add(operatorUser);
                        //    }
                        //}
                        operatorUsers = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, true, false);
                    }
                    else if(user.UserType == 5)
                    {
                        ClearFilters();
                        _plants = await PlantServices.GetPlants();
                        _plants = _plants.OrderBy(p => p.Description).ToList();

                        plantId = (int)user.PlantId;
                        _areas = await AreaServices.GetAreas(plantId);
                        _areas = _areas.OrderBy(a => a.Description).ToList();

                        foreach (var jobobs in _jobObservationsAux)
                        {
                            if (plantId == jobobs.PlantId)
                            {
                                foreach (User usr in user.Subordinates)
                                {
                                    if (jobobs.Supervisor.SuperiorId == usr.UserId)
                                    {
                                        if(jobobs.Status != 7)
                                        {
                                            _jobObservations.Add(jobobs);
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
                                        else
                                        {
                                            _SOSJobobservation.Add(jobobs);
                                        }
                                    }

                                }
                            }
                        }
                    }

                    _filterJobObservation = _jobObservations;
                    _filterPlannedJobObservation = _plannedJobObservation;
                    _filterInProgressJobObservation = _inProgressJobObservation;
                    _filterLateJobObservation = _lateJobObservation;
                    _filterUnderReviewJobObservation = _underReviewJobObservation;
                    _filterRejectedJobObservation = _rejectedJobObservation;
                    _filterFinishedJobObservation = _finishedJobObservation;
                    _filterProgrammedJobObservation = _SOSJobobservation;

                    totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
                    totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
                    totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
                    totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
                    totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
                    totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
                    totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";

                }
            }
        }

        private async void ShowAreas()
        {

            if (plantId == 0)
            {
                areaId = 0;
                ClearFilters();
                _jobObservations = new();
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (jobobs.Status != 7)
                    {
                        _jobObservations.Add(jobobs);
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
                    else
                    {
                        _SOSJobobservation.Add(jobobs);
                    }
                }
                _filterJobObservation = _jobObservations;
                _filterPlannedJobObservation = _plannedJobObservation;
                _filterInProgressJobObservation = _inProgressJobObservation;
                _filterLateJobObservation = _lateJobObservation;
                _filterUnderReviewJobObservation = _underReviewJobObservation;
                _filterRejectedJobObservation = _rejectedJobObservation;
                _filterFinishedJobObservation = _finishedJobObservation;
                _filterProgrammedJobObservation = _SOSJobobservation;

                totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
                totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
                totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
                totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
                totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
                totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
                totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";
                StateHasChanged();
                return;

            }

            areaId = 0;
            color = Color.Default;
            filters = false;
            ClearFilters();
            operatorUsers.Clear();
            _jobObservations.Clear();

            _plannedJobObservation.Clear();
            _inProgressJobObservation.Clear();
            _lateJobObservation.Clear();
            _underReviewJobObservation.Clear();
            _rejectedJobObservation.Clear();
            _finishedJobObservation.Clear();
            _SOSJobobservation.Clear();


            foreach (var jobobs in _jobObservationsAux)
            {
                if (plantId == jobobs.PlantId)
                {
                    if(jobobs.Status != 7)
                    {
                        _jobObservations.Add(jobobs);
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
                    else
                    {
                        _SOSJobobservation.Add(jobobs);
                    }
                }
            }

            _filterJobObservation = _jobObservations;
            _filterPlannedJobObservation = _plannedJobObservation;
            _filterInProgressJobObservation = _inProgressJobObservation;
            _filterLateJobObservation = _lateJobObservation;
            _filterUnderReviewJobObservation = _underReviewJobObservation;
            _filterRejectedJobObservation = _rejectedJobObservation;
            _filterFinishedJobObservation = _finishedJobObservation;
            _filterProgrammedJobObservation = _SOSJobobservation;

            totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
            totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
            totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
            totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";



            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();

            StateHasChanged();
        }


        private async void ShowJobObs()
        {
            if(areaId == 0)
            {
                ShowAreas();
                return;
            }

            ClearFilters();
            _jobObservations.Clear();
            operatorUsers.Clear();
            _plannedJobObservation.Clear();
            _inProgressJobObservation.Clear();
            _lateJobObservation.Clear();
            _underReviewJobObservation.Clear();
            _rejectedJobObservation.Clear();
            _finishedJobObservation.Clear();
            _SOSJobobservation.Clear();

            if(user.UserType == 1 || user.UserType == 6)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && areaId == jobobs.AreaId)
                    {
                        if(jobobs.Status != 7)
                        {
                            _jobObservations.Add(jobobs);
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
                        else
                        {
                            _SOSJobobservation.Add(jobobs);
                        }
                    }
                }
            }
            else if(user.UserType == 2)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.SupervisorId == usr.UserId && usr.AreaId == areaId)
                            {
                                if(jobobs.Status != 7)
                                {
                                    _jobObservations.Add(jobobs);
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
                                else
                                {
                                    _SOSJobobservation.Add(jobobs);
                                }
                            }
                        }
                    }
                }
            }
            else if(user.UserType == 3)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && jobobs.SupervisorId == user.UserId && user.AreaId == areaId)
                    {
                        if(jobobs.Status != 7)
                        {
                            _jobObservations.Add(jobobs);
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
                        else
                        {
                            _SOSJobobservation.Add(jobobs);
                        }
                    }
                }
            }
            else if(user.UserType == 5)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.Supervisor.SuperiorId == usr.UserId && jobobs.AreaId == areaId)
                            {
                                if(jobobs.Status != 7)
                                {
                                    _jobObservations.Add(jobobs);
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
                                else
                                {
                                    _SOSJobobservation.Add(jobobs);
                                }
                            }
                        }
                    }
                }
            }

            _filterJobObservation = _jobObservations;
            _filterPlannedJobObservation = _plannedJobObservation;
            _filterInProgressJobObservation = _inProgressJobObservation;
            _filterLateJobObservation = _lateJobObservation;
            _filterUnderReviewJobObservation = _underReviewJobObservation;
            _filterRejectedJobObservation = _rejectedJobObservation;
            _filterFinishedJobObservation = _finishedJobObservation;
            _filterProgrammedJobObservation = _SOSJobobservation;

            totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
            totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
            totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
            totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";

            _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);

            //operator User
            //users = await UsersService.GetUsers();
            //foreach (var operatorUser in users)
            //{
            //    if (operatorUser.PlantId == plantId && operatorUser.AreaId == areaId && operatorUser.UserType == 4)
            //    {
            //        operatorUsers.Add(operatorUser);
            //    }
            //}
            operatorUsers = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, true, false);

            StateHasChanged();

        }

        public void ActiveFilters()
        {
            if (!filters && areaId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Select an Area first", Severity.Warning);
                return;
            }
            filters = !filters;

            _jobObservations = _filterJobObservation;
            _plannedJobObservation= _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation= _filterUnderReviewJobObservation;
            _rejectedJobObservation= _filterRejectedJobObservation;
            _finishedJobObservation= _filterFinishedJobObservation;
            _SOSJobobservation = _filterProgrammedJobObservation;

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
            _jobObservations = _filterJobObservation;
            _plannedJobObservation = _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation = _filterUnderReviewJobObservation;
            _rejectedJobObservation = _filterRejectedJobObservation;
            _finishedJobObservation = _filterFinishedJobObservation;
            _SOSJobobservation = _filterProgrammedJobObservation;

            totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
            totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
            totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
            totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";

            idFilter = new();
            distributionId = new();
            operationFlag = false;
            operationId = new();
            filterDate = null;
            operatorId = new();
            statusId = new();

            StateHasChanged();
        }

        //Filters
        private void FilterDistributions()
        {
            operationId = new();
            _jobObservations = _filterJobObservation;
            _plannedJobObservation = _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation = _filterUnderReviewJobObservation;
            _rejectedJobObservation = _filterRejectedJobObservation;
            _finishedJobObservation = _filterFinishedJobObservation;
            _SOSJobobservation = _filterProgrammedJobObservation;

            operationFlag = true;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == distributionId)].Operations;
            _jobObservations = _jobObservations.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _finishedJobObservation = _filterFinishedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();

            if (statusId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.Status == statusId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
            }
            if (operatorId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
            }
            if (filterDate != null)
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
            }
            if (idFilter != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
            }

            totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
            totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
            totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
            totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";
        }

        private void Filters()
        {
            _jobObservations = _filterJobObservation;
            _plannedJobObservation = _filterPlannedJobObservation;
            _inProgressJobObservation = _filterInProgressJobObservation;
            _lateJobObservation = _filterLateJobObservation;
            _underReviewJobObservation = _filterUnderReviewJobObservation;
            _rejectedJobObservation = _filterRejectedJobObservation;
            _finishedJobObservation = _filterFinishedJobObservation;
            _SOSJobobservation = _filterProgrammedJobObservation;

            if (distributionId != default(int)) {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.DistributionId == distributionId).ToList();
            }
            if (operationId!= default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.OperationId == operationId).ToList();
            }
            if (statusId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.Status == statusId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.Status == statusId).ToList();
            }
            if (operatorId != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.OperatorId == operatorId).ToList();
            }
            if(filterDate != null)
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.StartDate?.ToShortDateString() == filterDate?.ToShortDateString()).ToList();
            }
            if(idFilter != default(int))
            {
                _jobObservations = _jobObservations.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _plannedJobObservation = _plannedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _inProgressJobObservation = _inProgressJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _lateJobObservation = _lateJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _underReviewJobObservation = _underReviewJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _rejectedJobObservation = _rejectedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _finishedJobObservation = _finishedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
                _SOSJobobservation = _filterProgrammedJobObservation.Where(jobObs => jobObs.JobObservationId == idFilter).ToList();
            }

            totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
            totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
            totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
            totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";
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
            _jobObservationsAux = await JobObservationService.GetAllJobObservations();

            foreach (var jobobs in _jobObservationsAux)
            {
                if(Convert.ToDateTime(jobobs.EndDate?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 6 && jobobs.Status != 3 && jobobs.Status != 7)
                {
                    jobobs.Status = 3;

                    await JobObservationService.UpdateJobObservation(jobobs, "S.M. System");
                }
            }
        }


        async Task DeleteJobObservation(int jobObservationId)
        {
            _jobObservations.RemoveAll(jobObservation => jobObservation.JobObservationId == jobObservationId);
            await JobObservationService.DeleteJobObservation(jobObservationId);
            ClearFilters();
            _plannedJobObservation.Clear();
            _inProgressJobObservation.Clear();
            _lateJobObservation.Clear();
            _underReviewJobObservation.Clear();
            _rejectedJobObservation.Clear();
            _finishedJobObservation.Clear();
            _SOSJobobservation.Clear();

            if (user.UserType == 1)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && areaId == jobobs.AreaId)
                    {
                        if (jobobs.Status != 7)
                        {
                            _jobObservations.Add(jobobs);
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
                        else
                        {
                            _SOSJobobservation.Add(jobobs);
                        }
                    }
                }
            }
            else if (user.UserType == 2)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.SupervisorId == usr.UserId && usr.AreaId == areaId)
                            {
                                if (jobobs.Status != 7)
                                {
                                    _jobObservations.Add(jobobs);
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
                                else
                                {
                                    _SOSJobobservation.Add(jobobs);
                                }
                            }
                        }
                    }
                }
            }
            else if (user.UserType == 3)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId && jobobs.SupervisorId == user.UserId && user.AreaId == areaId)
                    {
                        if (jobobs.Status != 7)
                        {
                            _jobObservations.Add(jobobs);
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
                        else
                        {
                            _SOSJobobservation.Add(jobobs);
                        }
                    }
                }
            }
            else if (user.UserType == 5)
            {
                foreach (var jobobs in _jobObservationsAux)
                {
                    if (plantId == jobobs.PlantId)
                    {
                        foreach (User usr in user.Subordinates)
                        {
                            if (jobobs.Supervisor.SuperiorId == usr.UserId && jobobs.AreaId == areaId)
                            {
                                if (jobobs.Status != 7)
                                {
                                    _jobObservations.Add(jobobs);
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
                                else
                                {
                                    _SOSJobobservation.Add(jobobs);
                                }
                            }
                        }
                    }
                }
            }


            totalPlanned = Localizer["planned"] + " (" + _plannedJobObservation.Count + ")";
            totalInProgress = Localizer["inProgress"] + " (" + _inProgressJobObservation.Count + ")";
            totalLate = Localizer["late"] + " (" + _lateJobObservation.Count + ")";
            totalUnderReview = Localizer["underReview"] + " (" + _underReviewJobObservation.Count + ")";
            totalRejected = Localizer["rejected"] + " (" + _rejectedJobObservation.Count + ")";
            totalFinished = Localizer["finished"] + " (" + _finishedJobObservation.Count + ")";
            totalProgrammed = Localizer["programmed"] + " (" + _SOSJobobservation.Count + ")";

            visibleDelete = false;

            StateHasChanged();
        }

        void EditJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        void CreateJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);

                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createjobobservation/{dateString}");
            }
        }

        void PlanJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);
                var formatedDate = date;

                var EnglishDate =  formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/planjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/planjobobservation/{dateString}");
            }
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

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

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
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        //Double clic go to details
        private DateTime lastTouchTime = DateTime.MinValue;
        private readonly TimeSpan doubleTouchInterval = TimeSpan.FromMilliseconds(300);

        private void HandleTouchStart(int jobObsId)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastTouch = now - lastTouchTime;

            if (timeSinceLastTouch < doubleTouchInterval)
            {
                OpenDialog2(jobObsId);
            }

            lastTouchTime = now;
        }

        private int selectedRowNumber = -1;
        private MudTable<JobObservation> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<JobObservation> tableRowClickEventArgs)
        {
        }


        private string SelectedRowClassFunc(JobObservation element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
