namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public class DiagramData
    {
        public List<NodeData> Nodes { get; set; } = new();
        public List<LinkData> Links { get; set; } = new();
    }

    public class NodeData
    {
        public string Id { get; set; }
        public PositionData Position { get; set; }
        public SizeData Size { get; set; } // Tamaño del nodo
        public string Type { get; set; }   // Tipo de nodo para restaurarlo
        public string Title { get; set; }  // Título del nodo
    }


    public class PositionData
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class SizeData
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class LinkData
    {
        public string Id { get; set; }
        public string SourceNodeId { get; set; }
        public string TargetNodeId { get; set; }
        public string SourcePort { get; set; } // Puerto de origen cual de los 4 es (Top, Bottom, Left, Right)
        public string TargetPort { get; set; }// Puerto Destino cual de los 4 es (Top, Bottom, Left, Right)
    }

}
