namespace NetBuild.App.Core.ApiModel
{
    public class System
    {
        public string Id { get; set; }
        public string BuildingId { get; set; } // Partition Key
        public string Name { get; set; }
        public IDictionary<string, string> Metadata {  get; set; }
        public IEnumerable<DataInput> DataInputs { get; set; }
    }

    public class DataInput { 
        public string Id { get; set; }
        public string Name { get; set; }
        public double? Value { get; set; }
        public long? Date { get; set; }
        public string Units { get; set; }
    }
}
