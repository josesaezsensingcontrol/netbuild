namespace NetBuild.App.Core.ApiModel.Responses.Buildings
{
    public class AddBuildingRequest
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
