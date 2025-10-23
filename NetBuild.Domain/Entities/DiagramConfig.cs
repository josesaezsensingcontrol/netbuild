namespace NetBuild.Domain.Entities
{
    public class DiagramConfig
    {
        public string? ImageUrl { get; set; }
        public IEnumerable<DiagramNode> Nodes { get; set; }
    }

    public class DiagramNode
    { 
        public string Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Expression { get; set; }
        public string Units { get; set; }
    }
}
