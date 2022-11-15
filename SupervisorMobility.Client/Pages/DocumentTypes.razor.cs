using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class DocumentTypes
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Support Document Types", href: "", disabled: true)
        };

        public List<SupportDocumentType> _supportDocumentTypes { get; set; } = new();

        SupportDocumentType _supportDocumentType = new();

        protected async override Task OnInitializedAsync()
        {
            _supportDocumentTypes = await SupportDocumentTypeService.GetSupportDocumentTypes();
        }

        void CreateSupportDocType()
        {
            NavigationManager.NavigateTo($"documenttypes/createsupportdoctype");
        }

        void EditSupportDocType(int supportDocumentTypeId)
        {
            NavigationManager.NavigateTo($"documenttypes/updatesupportdoctype/{supportDocumentTypeId}");
        }
    }
}
