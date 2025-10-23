namespace NetBuild.Domain.Entities
{
    public class DataInputEntry
    {
        public string Id { get; set; }
        public string DataId { get; set; } // Partition Key: Building Id + System Id + DataInput Id
        public double Value { get; set; }
        public long Date { get; set; }
    }
}
