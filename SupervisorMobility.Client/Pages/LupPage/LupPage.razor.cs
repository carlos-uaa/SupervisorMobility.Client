using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Services.LupService;

namespace SupervisorMobility.Client.Pages.LupPage
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


        private bool visible = false;
        private int lupId;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("LUP", href: "/lup", disabled: true),
        };

        //User
        private string json = string.Empty;
        public User user = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {

            await GetUserAsync();

            jobObservationList = await JobObservationServices.GetAllJobObservationsWithLup();
            foreach(var jobObs in jobObservationList)
            {
                if(jobObs.Lup.Count > 0)
                {
                    foreach (var lup in jobObs.Lup)
                    {
                        if(user != null && user.AreaId == jobObs.AreaId)
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


            //_lup = await LupServices.GetAllLup();
            
            //foreach(var lup in _lup)
            //{
            //    switch (lup.Pillar)
            //    {
            //        case 1: _lupS.Add(lup); break;
            //        case 2: _lupQ.Add(lup); break;
            //        case 3: _lupD.Add(lup); break;
            //        case 4: _lupC.Add(lup); break;
            //        case 5: _lupOther.Add(lup); break;
            //    }
            //}
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


        // Create product
        void CreateLup()
        {
            NavigationManager.NavigateTo($"lup/createlup");
        }


        // Delete product
        async Task DeleteLup(int lupId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this lup?");

            if (confirm)
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
            }
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

    }
}
