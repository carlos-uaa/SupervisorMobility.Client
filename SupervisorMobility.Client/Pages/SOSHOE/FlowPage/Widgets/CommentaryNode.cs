using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Positions;
using Blazor.Diagrams.Models;
using Microsoft.AspNetCore.Components.Web;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets
{
    public class CommentaryNode : SvgNodeModel, ICmpAnchors
    {
        public CommentaryNode(double width, double height, Point? position = null) : base(position) 
        {
            Width = width;
            Height = height;

            double boundsWh =(width/2)/(width + 20);
            double boundsHh =(height/2)/(height + 20);
            double boundsWt =(width)/(width + 20);
            double boundsHt =(height)/(height + 20);

            Anchors = new List<DynamicAnchor>
            {
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(0,boundsHh) }),   // Left-center
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(boundsWh,0) }),   // Top-center
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(boundsWt,boundsHh) }),   // Right-center
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(boundsWh,boundsHt) }),   // Bottom-center
            };
        }
        public double Width { get; set; }
        public double Height { get; set; }
        public List<DynamicAnchor> Anchors { get; set; }
        public bool IsResizing { get; set; } = false;
        public bool IsEditing { get; set; } = false;
        public void HideLinks()
        {
            foreach(var anchor in Anchors)
            {
                foreach (var link in anchor.Model.Links)
                {
                    link.Visible = false;
                }
            }
        }

        public void ReubicateAnchors()
        {
            double boundsWh = (Width / 2) / (Width + 20);
            double boundsHh = (Height / 2) / (Height + 20);
            double boundsWt = (Width) / (Width + 20);
            double boundsHt = (Height) / (Height + 20);

            var OldAnchors = Anchors;

            Anchors = new List<DynamicAnchor>
            {
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(0,boundsHh) }),   // Left-center
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(boundsWh,0) }),   // Top-center
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(boundsWt,boundsHh) }),   // Right-center
                new DynamicAnchor(this, new []{ new BoundsBasedPositionProvider(boundsWh,boundsHt) }),   // Bottom-center
            };

            for (int i = 0; i < OldAnchors.Count && i < Anchors.Count; i++)
            {
                var oldAnchor = OldAnchors[i];
                var newAnchor = Anchors[i];

                foreach (var link in oldAnchor.Model.Links.ToList())
                {
                    if (link.Source == oldAnchor)
                        link.SetSource(newAnchor);

                    if (link.Target == oldAnchor)
                        link.SetTarget(newAnchor);

                    link.Refresh();
                }
            }
        }

        public void ShowLinks()
        {
            foreach (var anchor in Anchors)
            {
                foreach (var link in anchor.Model.Links)
                {
                    link.Visible = true;
                }
            }
        }

        public async Task HandleKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                IsEditing=false;
            }
        }
    }
}
