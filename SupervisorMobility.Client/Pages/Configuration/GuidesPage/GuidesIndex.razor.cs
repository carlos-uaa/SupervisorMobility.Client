using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GuidesPage
{
    public partial class GuidesIndex
    {
        List<Guide> _Guides =  new List<Guide>();
        string searchString = "";

        //Breadcrumb links
        private List<BreadcrumbItem> _links;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: @Localizer["home"], href: "#"),
            new BreadcrumbItem(text: @Localizer["GuidesTitle"], href: "", disabled: true)
        };
            _Guides = await GuidesServices.GetAllGuidesWhitFile();
        }

        private bool FilterFunc(Guide element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.GuideId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.GuideId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }


        private async Task DownloadFile(int fileId, string filename)
        {
            await FileServices.DownloadFileGuide(fileId, filename);
        }

    }
}