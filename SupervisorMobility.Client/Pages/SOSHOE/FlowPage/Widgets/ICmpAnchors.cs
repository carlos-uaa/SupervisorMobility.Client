using Blazor.Diagrams.Core.Anchors;
using Microsoft.AspNetCore.Components.Web;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets
{
    public interface ICmpAnchors
    {
        public List<DynamicAnchor> Anchors { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsResizing { get; set; }
        public bool IsEditing { get; set; }

        public void ReubicateAnchors();
        public void HideLinks();
        public void ShowLinks();
        public Task HandleKey(KeyboardEventArgs e);
    }
}
