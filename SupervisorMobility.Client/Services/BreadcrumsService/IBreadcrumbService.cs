
using MudBlazor;

namespace SupervisorMobility.Client.Services.BreadcrumsService
{
    public interface IBreadcrumbService
    {
        event Action<List<BreadcrumbItem>> BreadcrumbsChanged;
        List<BreadcrumbItem> Breadcrumbs { get; }

        void UpdateBreadcrumbs(List<BreadcrumbItem> breadcrumbs);
    }
}
