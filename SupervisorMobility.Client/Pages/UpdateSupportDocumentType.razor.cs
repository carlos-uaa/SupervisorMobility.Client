using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateSupportDocumentType
    {
        [Parameter]
        public int SupportDocumentTypeId { get; set; }

        public SupportDocumentType _supportDocumentType { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Support Document Types", href: "/documenttypes"),
            new BreadcrumbItem("UpdateDocType", href: "", disabled: true)
        };

        protected override async Task OnParametersSetAsync()
        {
            SupportDocumentType dbSupportDocumentType = await SupportDocumentTypeService.GetSupportDocumentTypeById(SupportDocumentTypeId);
            _supportDocumentType = dbSupportDocumentType;
        }

        void UpdateSupportDocumentTypeAsync()
        {
            SupportDocumentTypeService.UpdateSupportDocumentType(_supportDocumentType);
            NavigationManager.NavigateTo($"documenttypes");
        }
    }
}
