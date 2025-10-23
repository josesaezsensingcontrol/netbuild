
namespace NetBuild.Domain.Entities
{
    public class Building
    {
        public string Id { get; set; }
        public string NeighborhoodId { get; set; } // Partition Key 0 
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DiagramConfig? Diagram { get; set; }
        public Schedule? Schedule { get; set; }
        public WeatherForecast? WeatherForecast { get; set; }
    }
}
