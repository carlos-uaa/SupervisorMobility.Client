using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.LupService;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupPage
    {

        public List<JobObservation> jobObservationList { get; set; }

        public List<Lup> _lup { get; set; } = new();
        public List<Lup> _lupS { get; set; } = new();
        public List<Lup> _lupQ { get; set; } = new();
        public List<Lup> _lupD { get; set; } = new();
        public List<Lup> _lupC { get; set; } = new();
        public List<Lup> _lupOther { get; set; } = new();

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

                jobObservationList = await JobObservationServices.GetAllJobObservationsWithLup();

                if(user != null)
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
                                    _lup.Add(lup);
                                    switch (lup.Pillar)
                                    {
                                        case 1: _lupS.Add(lup); break;
                                        case 2: _lupQ.Add(lup); break;
                                        case 3: _lupD.Add(lup); break;
                                        case 4: _lupC.Add(lup); break;
                                        case 5: _lupOther.Add(lup); break;
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
                                        _lup.Add(lup);
                                        switch (lup.Pillar)
                                        {
                                            case 1: _lupS.Add(lup); break;
                                            case 2: _lupQ.Add(lup); break;
                                            case 3: _lupD.Add(lup); break;
                                            case 4: _lupC.Add(lup); break;
                                            case 5: _lupOther.Add(lup); break;
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
                                        _lup.Add(lup);
                                        switch (lup.Pillar)
                                        {
                                            case 1: _lupS.Add(lup); break;
                                            case 2: _lupQ.Add(lup); break;
                                            case 3: _lupD.Add(lup); break;
                                            case 4: _lupC.Add(lup); break;
                                            case 5: _lupOther.Add(lup); break;
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
                                        _lup.Add(lup);
                                        switch (lup.Pillar)
                                        {
                                            case 1: _lupS.Add(lup); break;
                                            case 2: _lupQ.Add(lup); break;
                                            case 3: _lupD.Add(lup); break;
                                            case 4: _lupC.Add(lup); break;
                                            case 5: _lupOther.Add(lup); break;
                                        }

                                    }
                                }
                            }
                        }
                    }

                }

            }


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
                            _lup.Add(lup);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(lup); break;
                                case 2: _lupQ.Add(lup); break;
                                case 3: _lupD.Add(lup); break;
                                case 4: _lupC.Add(lup); break;
                                case 5: _lupOther.Add(lup); break;
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
                            _lup.Add(lup);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(lup); break;
                                case 2: _lupQ.Add(lup); break;
                                case 3: _lupD.Add(lup); break;
                                case 4: _lupC.Add(lup); break;
                                case 5: _lupOther.Add(lup); break;
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
                            _lup.Add(lup);
                            switch (lup.Pillar)
                            {
                                case 1: _lupS.Add(lup); break;
                                case 2: _lupQ.Add(lup); break;
                                case 3: _lupD.Add(lup); break;
                                case 4: _lupC.Add(lup); break;
                                case 5: _lupOther.Add(lup); break;
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
            _lup.RemoveAll(l => l.LupId == lupId);
            await LupServices.DeleteLup(lupId);

            _lup = await LupServices.GetAllLup();
            _lupS.Clear();
            _lupQ.Clear();
            _lupD.Clear();
            _lupC.Clear();
            _lupOther.Clear();

            foreach (var lup in _lup)
            {
                switch (lup.Pillar)
                {
                    case 1: _lupS.Add(lup); break;
                    case 2: _lupQ.Add(lup); break;
                    case 3: _lupD.Add(lup); break;
                    case 4: _lupC.Add(lup); break;
                    case 5: _lupOther.Add(lup); break;
                }
            }
            visibleDelete = false;

        }

        // Update product
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        private string searchString = "";

        private bool FilterFunc(Lup element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.LupId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Oportunity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.LupId} {element.Status} {element.Observer}".Contains(searchString))
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
        private MudTable<Lup> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Lup> tableRowClickEventArgs)
        {
        }


        private string SelectedRowClassFunc(Lup element, int rowNumber)
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
