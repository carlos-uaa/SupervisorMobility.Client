using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.DocumentTypePage
{
    public partial class UpdateSupportDocumentType
    {
        // Parameters
        [Parameter]
        public int SupportDocumentTypeId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        public SupportDocumentType _supportDocumentType { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            SupportDocumentType dbSupportDocumentType = await SupportDocumentTypeService.GetSupportDocumentTypeById(SupportDocumentTypeId);
            _supportDocumentType = dbSupportDocumentType;
            _links = new List<BreadcrumbItem>
        {
              new BreadcrumbItem(text: Localizer["home"], href: "#"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["JOTTitle"], href: "/documenttypes"),
            new BreadcrumbItem(text: Localizer["JOTUpdate"], href: "", disabled: true)
        };
        }

        // Update support document type
        async void UpdateSupportDocumentTypeAsync()
        {

            var result = await SupportDocumentTypeService.UpdateSupportDocumentType(_supportDocumentType);

            if (result)
            {
                NavigationManager.NavigateTo($"documenttypes");
            }

        }
    }
}
