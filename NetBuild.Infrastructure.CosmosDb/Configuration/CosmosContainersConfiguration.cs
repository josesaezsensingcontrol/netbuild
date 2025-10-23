namespace NetBuild.Infrastructure.CosmosDb.Configuration
{
    public class CosmosContainersConfiguration
    {
        public string UserContainerName { get; set; }
        public string NeighborhoodContainerName { get; set; }
        public string BuildingContainerName { get; set; }
        public string SystemContainerName { get; set; }
        public string HistoricDataContainerName { get; set; }
        public string PredictionDataContainerName { get; set; }
    }
}
