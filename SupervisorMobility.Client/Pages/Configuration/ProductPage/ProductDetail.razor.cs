using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class ProductDetail
    {
        [Parameter]
        public int ProductId { get; set; }


        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "", disabled: true),
            new BreadcrumbItem("Products Detail", href: "", disabled: true),
        };

        // Objects
        Plant _plant = new();
        private List<Area> _areas = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(ProductId);
            _areas = await AreaService.GetAreas(ProductId);
        }

        // Area details
        void AreaDetails(int plantId, int areaId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}/areas/{areaId}");
        }

        // Create area
        void CreateArea(int PlantId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/createarea");
        }

        // Delete area
        async Task DeleteArea(int areaId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this area?");

            if (confirm)
            {
                _areas.RemoveAll(area => area.AreaId == areaId);
                await AreaService.DeleteArea(ProductId, areaId);
            }
        }

        // Update area
        void UpdateArea(int plantId, int areaId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}/updatearea/{areaId}");
        }


    }
}
