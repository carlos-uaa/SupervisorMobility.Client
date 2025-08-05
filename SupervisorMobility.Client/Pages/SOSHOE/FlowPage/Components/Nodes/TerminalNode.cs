using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Components.Nodes
{
    public class TerminalNode : NodeModel
    {
        public TerminalNode(Point position = null) : base(position) { }

        public string Answer { get; set; }

    }
}
