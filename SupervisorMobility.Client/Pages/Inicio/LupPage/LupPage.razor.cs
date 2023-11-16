using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.LupService;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupPage
    {

        public List<JobObservation> jobObservationList { get; set; }

        public List<LupWithDistribution> _lup { get; set; } = new();
        public List<LupWithDistribution> _lupS { get; set; } = new();
        public List<LupWithDistribution> _lupQ { get; set; } = new();
        public List<LupWithDistribution> _lupD { get; set; } = new();
        public List<LupWithDistribution> _lupC { get; set; } = new();
        public List<LupWithDistribution> _lupOther { get; set; } = new();

        LupWithDistribution LupAux = new();
        //Admin
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();

        public int plantId;
        public int areaId;

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

                jobObservationList = await JobObservationServices.GetAllJobObservations(true, true, true, false, false);

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
                            _lup.Add(LupAux);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(LupAux); break;
                                case 2: _lupQ.Add(LupAux); break;
                                case 3: _lupD.Add(LupAux); break;
                                case 4: _lupC.Add(LupAux); break;
                                case 5: _lupOther.Add(LupAux); break;
                            }
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

                                _lup.Add(LupAux);
                                switch (lup.Pillar)
                                {
                                    case 1: _lupS.Add(LupAux); break;
                                    case 2: _lupQ.Add(LupAux); break;
                                    case 3: _lupD.Add(LupAux); break;
                                    case 4: _lupC.Add(LupAux); break;
                                    case 5: _lupOther.Add(LupAux); break;
                                }

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

                                _lup.Add(LupAux);
                                switch (lup.Pillar)
                                {
                                    case 1: _lupS.Add(LupAux); break;
                                    case 2: _lupQ.Add(LupAux); break;
                                    case 3: _lupD.Add(LupAux); break;
                                    case 4: _lupC.Add(LupAux); break;
                                    case 5: _lupOther.Add(LupAux); break;
                                }

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

                                _lup.Add(LupAux);
                                switch (lup.Pillar)
                                {
                                    case 1: _lupS.Add(LupAux); break;
                                    case 2: _lupQ.Add(LupAux); break;
                                    case 3: _lupD.Add(LupAux); break;
                                    case 4: _lupC.Add(LupAux); break;
                                    case 5: _lupOther.Add(LupAux); break;
                                }

                            }
                        }
                    }
                }
            }
            StateHasChanged();
        }

        private async void ShowAreas()
        {
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

                            _lup.Add(LupAux);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(LupAux); break;
                                case 2: _lupQ.Add(LupAux); break;
                                case 3: _lupD.Add(LupAux); break;
                                case 4: _lupC.Add(LupAux); break;
                                case 5: _lupOther.Add(LupAux); break;
                            }
                        }
                    }
                }
                StateHasChanged();
                return;
            }

                _lup.Clear();
            _lupS.Clear();
            _lupQ.Clear();
            _lupD.Clear();
            _lupC.Clear();
            _lupOther.Clear();

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

                            _lup.Add(LupAux);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(LupAux); break;
                                case 2: _lupQ.Add(LupAux); break;
                                case 3: _lupD.Add(LupAux); break;
                                case 4: _lupC.Add(LupAux); break;
                                case 5: _lupOther.Add(LupAux); break;
                            }

                        }
                    }
                }
            }

            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();

            StateHasChanged();
        }

        private void ShowLups()
        {
            if (areaId == 0)
            {
                ShowAreas();
                return;
            }
            _lup.Clear();
            _lupS.Clear();
            _lupQ.Clear();
            _lupD.Clear();
            _lupC.Clear();
            _lupOther.Clear();

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

                            _lup.Add(LupAux);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(LupAux); break;
                                case 2: _lupQ.Add(LupAux); break;
                                case 3: _lupD.Add(LupAux); break;
                                case 4: _lupC.Add(LupAux); break;
                                case 5: _lupOther.Add(LupAux); break;
                            }

                        }
                    }
                }
            }
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
    }
}
