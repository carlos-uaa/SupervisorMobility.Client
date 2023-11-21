using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.LupService;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupPage
    {

        public List<JobObservation> jobObservationList { get; set; }

        public List<LupWithDistribution> _lups { get; set; } = new();
        public List<LupWithDistribution> _filterlups { get; set; } = new();


        LupWithDistribution LupAux = new();
        //Admin
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();

        public int plantId;
        public int areaId;

        //Filters

        public Color color = Color.Default;
        public bool filters = false;
        List<Distribution> _distributions = new();
        List<Department> _departments = new();
        public int distributionId;
        public int statusId;
        public int departmentId;


        private bool visible = false;
        private int lupId;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        // Initialization
        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem("LUP", href: "/lup", disabled: true),
            };

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["errorYouHaveToLogIn"], Severity.Warning);
                NavigationManager.NavigateTo($"/");
            }
            else
            {

                await GetUserAsync();

                jobObservationList = await JobObservationServices.GetAllJobObservationsWithLup();
                _departments = await DepartmentServices.GetDepartments();

                if (user != null)
                {
                    GetLupByUser();
                }

            }


        }

        private async void GetLupByUser()
        {
            if (user.UserType == 1 || user.UserType == 6)
            {
                _plants = await PlantServices.GetPlants();
                _plants = _plants.OrderBy(p => p.Description).ToList();

                foreach (var jobObs in jobObservationList)
                {
                    if (jobObs.Lup.Count > 0)
                    {
                        foreach (var lup in jobObs.Lup)
                        {
                            LupAux = new();
                            LupAux.Lup = lup;
                            LupAux.Distribution = jobObs.Distribution?.Description;
                            LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                            _lups.Add(LupAux);
                        }
                    }
                }
            }
            else if (user.UserType == 2)
            {
                plantId = (int)user.PlantId;
                if (user.Areas != null)
                {
                    _areas = user.Areas.ToList();
                    _areas.OrderBy(a => a.Description).ToList();
                }


                foreach (var jobObs in jobObservationList)
                {
                    if (jobObs.Lup.Count > 0)
                    {
                        foreach (var lup in jobObs.Lup)
                        {
                            if (plantId == jobObs.PlantId)
                            {
                                LupAux = new();
                                LupAux.Lup = lup;
                                LupAux.Distribution = jobObs.Distribution?.Description;
                                LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                                _lups.Add(LupAux);
                            }
                        }
                    }
                }
            }
            else if (user.UserType == 3)
            {
                plantId = (int)user.PlantId;
                areaId = (int)user.AreaId;
                foreach (var jobObs in jobObservationList)
                {
                    if (jobObs.Lup.Count > 0)
                    {
                        foreach (var lup in jobObs.Lup)
                        {
                            if (plantId == jobObs.PlantId && areaId == jobObs.AreaId)
                            {
                                LupAux = new();
                                LupAux.Lup = lup;
                                LupAux.Distribution = jobObs.Distribution?.Description;
                                LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                                _lups.Add(LupAux);

                            }
                        }
                    }
                }

            }
            else if (user.UserType == 5)
            {
                plantId = (int)user.PlantId;
                areaId = 0;
                _areas = await AreaServices.GetAreas(plantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();

                foreach (var jobObs in jobObservationList)
                {
                    if (jobObs.Lup.Count > 0)
                    {
                        foreach (var lup in jobObs.Lup)
                        {
                            if (plantId == jobObs.PlantId)
                            {
                                LupAux = new();
                                LupAux.Lup = lup;
                                LupAux.Distribution = jobObs.Distribution?.Description;
                                LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                                _lups.Add(LupAux);

                            }
                        }
                    }
                }
            }

            _filterlups = _lups;
            StateHasChanged();
        }

        private async void ShowAreas()
        {

            _lups = new();
            if (plantId == 0)
            {
                foreach (var jobObs in jobObservationList)
                {
                    if (jobObs.Lup.Count > 0)
                    {
                        foreach (var lup in jobObs.Lup)
                        {
                            LupAux = new();
                            LupAux.Lup = lup;
                            LupAux.Distribution = jobObs.Distribution?.Description;
                            LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                            _lups.Add(LupAux);
                        }
                    }
                }
                areaId = 0;
                _filterlups = _lups;
                StateHasChanged();
                return;
            }


            foreach (var jobObs in jobObservationList)
            {
                if (jobObs.Lup.Count > 0)
                {
                    foreach (var lup in jobObs.Lup)
                    {
                        if (plantId == jobObs.PlantId)
                        {
                            LupAux = new();
                            LupAux.Lup = lup;
                            LupAux.Distribution = jobObs.Distribution?.Description;
                            LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                            _lups.Add(LupAux);

                        }
                    }
                }
            }

            _filterlups = _lups;
            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();

            StateHasChanged();
        }

        private async void ShowLups()
        {
            if (areaId == 0)
            {
                ShowAreas();
                return;
            }
            _lups.Clear();


            foreach (var jobObs in jobObservationList)
            {
                if (jobObs.Lup.Count > 0)
                {
                    foreach (var lup in jobObs.Lup)
                    {
                        if (areaId == jobObs.AreaId)
                        {
                            LupAux = new();
                            LupAux.Lup = lup;
                            LupAux.Distribution = jobObs.Distribution?.Description;
                            LupAux.DistributionId = jobObs.Distribution?.DistributionId;
                            _lups.Add(LupAux);

                        }
                    }
                }
            }
            _filterlups = _lups;
            _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);
            StateHasChanged();

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

        // Delete lup
        async Task DeleteLup(int lupId)
        {
            await LupServices.DeleteLup(lupId);

            GetLupByUser();

            visibleDelete = false;

        }

        // Update product
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        private string searchString = "";

        private bool FilterFunc(LupWithDistribution element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Lup.LupId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Lup.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Lup.Oportunity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.Lup.LupId} {element.Lup.Status} {element.Lup.Observer}".Contains(searchString))
                return true;
            return false;
        }

        //Modal

        private void OpenDialog2(int id)
        {
            lupId = id;
            visible = true;
        }
        void Close() => visible = false;



        //Delete lup
        private bool visibleDelete = false;
        public int deleteLupId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteLupId = deleteId;
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
        private MudTable<LupWithDistribution> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<LupWithDistribution> tableRowClickEventArgs)
        {
        }


        private string SelectedRowClassFunc(LupWithDistribution element, int rowNumber)
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


        //Filters
        private void Filters()
        {
            _filterlups = new();
            _filterlups = _lups;

            if(distributionId != default(int))
            {

                _filterlups = _filterlups.Where(l =>  l.DistributionId == distributionId).ToList();
            }
            if (statusId != default(int))
            {
                _filterlups = _filterlups.Where(l => l.Lup.Status == statusId).ToList();

            }
            if (departmentId != default(int))
            {
                _filterlups = _filterlups.Where(l => l.Lup.DepartmentId == departmentId).ToList();

            }



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



            if (color == Color.Info)
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
            _filterlups = _lups;
            distributionId = new();
            departmentId = new();
            statusId = new();

            StateHasChanged();
        }


    }
}
