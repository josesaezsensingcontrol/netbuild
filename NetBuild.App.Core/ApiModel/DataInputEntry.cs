namespace NetBuild.App.Core.ApiModel
{
    public class DataInputEntry
    {
        public string Id { get; set; }
        public string DataId { get; set; } // Partition Key: System Id + DataInput Id
        public double Value { get; set; }
        public long Date { get; set; }
    }
}
