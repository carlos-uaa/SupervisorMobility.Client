using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.DocumentTypePage
{
    public partial class CreateSupportDocumentType
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "/"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["SDTTitle"], href: "/jobobservationtypes"),
            new BreadcrumbItem(text: Localizer["SDTCreate"], href: "", disabled: true)
        };
        }

        // Objects
        SupportDocumentType _supportDocumentType = new();

        // Create support document type
        async void CreateSupportDocumentTypeAsync()
        {
            _supportDocumentType.IsActive = true;
            var result = await SupportDocumentTypeService.CreateSupportDocumentType(_supportDocumentType);
            NavigationManager.NavigateTo($"documenttypes");
        }
    }
}
