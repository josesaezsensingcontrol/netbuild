namespace NetBuild.App.Core.ApiModel.Responses.Systems
{
    public class AddSystemRequest
    {
        public string? SystemId { get; set; }
        public string BuildingId { get; set; }
        public string Name { get; set; }
        public IEnumerable<DataInput> DataInputs { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}
