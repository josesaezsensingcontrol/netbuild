namespace NetBuild.Domain.Entities
{
    public class Neighborhood
    {
        public string Id { get; set; }  // Partition Key
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
