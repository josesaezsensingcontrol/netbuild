using NetBuild.Domain.Types;

namespace NetBuild.App.Core.ApiModel.Responses.Ingestion
{
    public class IngestBulkDataRequest
    {
        public IDictionary<string, List<DataPoint>> Data { get; set; }
    }
}
