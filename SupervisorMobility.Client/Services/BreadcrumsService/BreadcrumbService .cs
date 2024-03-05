
using MudBlazor;

namespace SupervisorMobility.Client.Services.BreadcrumsService
{
    public class BreadcrumbService : IBreadcrumbService
    {
        public event Action<List<BreadcrumbItem>> BreadcrumbsChanged;

        private List<BreadcrumbItem> _breadcrumbs = new List<BreadcrumbItem>();
        public List<BreadcrumbItem> Breadcrumbs => _breadcrumbs;

        public void UpdateBreadcrumbs(List<BreadcrumbItem> breadcrumbs)
        {
            _breadcrumbs = breadcrumbs;
            BreadcrumbsChanged?.Invoke(_breadcrumbs);
        }
    }
}
