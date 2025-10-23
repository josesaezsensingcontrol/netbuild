using Newtonsoft.Json;

namespace NetBuild.App.Core.Electricity
{
    public class ReeResponse
    {
        public ReeData Data { get; set; }
        public List<ReeDataIncluded> Included { get; set; }
    }

    public class ReeData
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public ReeDataAttributes Attributes { get; set; }
        public ReeDataMeta meta { get; set; }
    }

    public class ReeDataAttributes
    {
        public string title { get; set; }

        [JsonProperty("last-update")]
        public DateTimeOffset LastUpdate { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string Magnitude { get; set; }
        public bool Composite { get; set; }
        public List<ReeValue> Values { get; set; }
    }

    public class ReeDataIncluded
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string GroupId { get; set; }
        public ReeDataAttributes Attributes { get; set; }
    }

    public class ReeDataMeta
    {
        [JsonProperty("cache-control")]
        public CacheControl CacheControl { get; set; }
    }

    public class CacheControl
    {
        public string Cache { get; set; }
    }

    public class ReeValue
    {
        public double Value { get; set; }
        public double Percentage { get; set; }
        public DateTimeOffset Datetime { get; set; }
    }
}
