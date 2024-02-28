using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.BreadcrumsService;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage
{
    public partial class AreaDetail
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();
        public List<Distribution> _distributions { get; set; } = new();
     
        //protected async override Task OnInitializedAsync()
        //{
        //     _links = new List<BreadcrumbItem>
        //    {
        //        new BreadcrumbItem(text: Localizer["home"], href: "/"),
        //        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
        //        new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
        //        new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
        //        new BreadcrumbItem(text: Localizer["areaDetails"], href: "", disabled: true)
        //    };
        //}

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _area = await AreaService.GetOneAreaIncludingCollections(PlantId, AreaId);
            _plant = await PlantService.GetPlantById(PlantId);
            _distributions = await DistributionService.GetDistributions(PlantId, AreaId);
            _links = new List<BreadcrumbItem>
              {
                  new BreadcrumbItem(text: Localizer["home"], href: "/"),
                  new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                  new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                  new BreadcrumbItem(text: _plant.Description, href: $"plants/{PlantId}"),
                  new BreadcrumbItem(text: _area.Description, href: $"plants/{PlantId}/areas/{AreaId}", disabled: true)
              };
              BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        // Links
        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void FunUpdateArea(int plantId, int areaId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}/updatearea/{areaId}");
        }


        // Create distribution
        void CreateDistribution()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/createdistribution");
        }

        // Delete distribution
        async Task DeleteDistribution(int distributionId)
        {
            _distributions.RemoveAll(distribution => distribution.DistributionId == distributionId);
            await DistributionService.DeleteDistribution(PlantId, AreaId, distributionId);

            visibleDelete = false;
        }

        // Distribution details
        void DistributionDetails(int distributionId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{distributionId}");
        }

        // Update distribution
        void UpdateDistribution(int distributionId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/updatedistribution/{distributionId}");
        }

        //Delete Distribution 
        private bool visibleDelete = false;
        public int deleteDistributionId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteDistributionId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        private int selectedRowNumber = -1;
        private MudTable<Distribution> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Distribution> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(Distribution element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                    NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{element.DistributionId}");

                }
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
