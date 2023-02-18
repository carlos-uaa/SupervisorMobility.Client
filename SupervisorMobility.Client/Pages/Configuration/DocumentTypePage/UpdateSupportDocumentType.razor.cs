using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.DocumentTypePage
{
    public partial class UpdateSupportDocumentType
    {
        // Parameters
        [Parameter]
        public int SupportDocumentTypeId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Support Document Types", href: "/documenttypes"),
            new BreadcrumbItem("UpdateDocType", href: "", disabled: true)
        };

        // Objects
        public SupportDocumentType _supportDocumentType { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            SupportDocumentType dbSupportDocumentType = await SupportDocumentTypeService.GetSupportDocumentTypeById(SupportDocumentTypeId);
            _supportDocumentType = dbSupportDocumentType;
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
