namespace NetBuild.App.Core.ApiModel.Responses.Buildings
{
    public class UpdateBuildingDiagramNodesRequest
    {
        public string Id { get; set; }
        public IEnumerable<DiagramNode> Nodes { get; set; }
    }
}
