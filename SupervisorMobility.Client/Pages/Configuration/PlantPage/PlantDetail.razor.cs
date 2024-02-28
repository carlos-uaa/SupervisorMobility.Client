using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage
{
    public partial class PlantDetail
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        private List<Area> _areas = new();

        // Initialization
        //protected async override Task OnInitializedAsync()
        //{
        //    _links = new List<BreadcrumbItem>
        //    {
        //        new BreadcrumbItem(text: Localizer["home"], href: "/"),
        //        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
        //        new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
        //        new BreadcrumbItem(text: Localizer["plantDetails"], href: "", disabled: true)
        //    }; 
        //}



        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _areas = await AreaService.GetAreas(PlantId);
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: _plant.Description, href: $"plants/{PlantId}", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
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
            _areas.RemoveAll(area => area.AreaId == areaId);
            await AreaService.DeleteArea(PlantId, areaId);

            visibleDelete = false;
        }

        // Update area
        void UpdateArea(int plantId, int areaId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}/updatearea/{areaId}");
        }


        //Delete Area 
        private bool visibleDelete = false;
        public int deleteAreaId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteAreaId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        private int selectedRowNumber = -1;
        private MudTable<Area> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Area> tableRowClickEventArgs)
        {
        }

        void FuncUpdatePlant(int plantId)
        {
            NavigationManager.NavigateTo($"plants/updateplant/{plantId}");
        }
        private string SelectedRowClassFunc(Area element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                    NavigationManager.NavigateTo($"plants/{PlantId}/areas/{element.AreaId}");
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
