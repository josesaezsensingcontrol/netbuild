using NetBuild.Domain.Types;

namespace NetBuild.App.Core.ApiModel.Responses.Ingestion
{
    public class IngestDataRequest
    {
        public IDictionary<string, DataPoint> Data { get; set; }
    }
}
