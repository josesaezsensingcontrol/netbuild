namespace NetBuild.App.Core.ApiModel.SignalR
{
    public class NewDataMessage
    {
        public string BuildingId { get; set; }
        public string SystemId { get; set; }
        public IDictionary<string, TimestampedValue> DataInputs { get; set; }
    }

    public class TimestampedValue { 
        public long Date { get; set; }
        public double Value { get; set; }
    }
}
