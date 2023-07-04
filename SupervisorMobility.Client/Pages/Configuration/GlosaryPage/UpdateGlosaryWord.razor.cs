using Microsoft.AspNetCore.Components;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.GlosaryPage
{
    public partial class UpdateGlosaryWord
    {
        // Parameters
        [Parameter]
        public int GlosaryWordId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        // Objects
        public Glosary _glosary { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {

            _links = new List<BreadcrumbItem>{
            new BreadcrumbItem(text: Localizer["home"], href: "/", disabled:false),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration", disabled:false),
            new BreadcrumbItem(text: Localizer["GlosaryTitle"], href: "/glosary", disabled: false),
            new BreadcrumbItem(text: Localizer["GlosaryUpdateTitle"], href: "", disabled: true)
            };
            Glosary dbGlosary = await GlosaryService.GetGlosaryWordbyId(GlosaryWordId);
            _glosary = dbGlosary;
        }
        async void UpdateGlosaryWordAsync()
        {

            var result = await GlosaryService.UpdateGlosaryWord(_glosary);

            if (result)
            {
                NavigationManager.NavigateTo($"glosary");
            }
        }
    }
}
