using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Services.LupService;

namespace SupervisorMobility.Client.Pages.LupPage
{
    public partial class LupPage
    {

        public List<Lup> _lup { get; set; } = new();
        public List<Lup> _lupS { get; set; } = new();
        public List<Lup> _lupQ { get; set; } = new();
        public List<Lup> _lupD { get; set; } = new();
        public List<Lup> _lupC { get; set; } = new();
        public List<Lup> _lupOther { get; set; } = new();


        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("LUP", href: "/lup", disabled: true),
        };

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _lup = await LupService.GetAllLup();
            
            foreach(var lup in _lup)
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
                _lup.RemoveAll(product => product.LupId == lupId);
                await LupService.DeleteLup(lupId);
            }
        }

        // Update product
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        void LupDetails(int lupId)
        {
            NavigationManager.NavigateTo($"lup/{lupId}");
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
            if (element.Observer.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.LupId} {element.Status} {element.Observer}".Contains(searchString))
                return true;
            return false;
        }

    }
}
