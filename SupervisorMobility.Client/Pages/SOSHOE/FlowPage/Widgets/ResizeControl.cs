using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Controls;
using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets
{
    public class ResizeControl : ExecutableControl
    {
        public override Point? GetPosition(Model model)
        {
            var node = (model as NodeModel)!;
            if (node.Size == null)
                return null;

            return node.Position.Add(-15, -15);
        }

        public override ValueTask OnPointerDown(Diagram diagram, Model model, PointerEventArgs e)
        {
            return ValueTask.CompletedTask;
        }
    }
}
