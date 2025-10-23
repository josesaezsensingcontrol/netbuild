namespace NetBuild.Infrastructure.CosmosDb.Configuration
{
    public class CosmosDbConfiguration
    {
        public string AccountEndpoint { get; set; }
        public string AccountKey { get; set; }
        public string DatabaseName { get; set; }
        public bool CreateIfNotExistsOnStartup { get; set; }
    }
}
