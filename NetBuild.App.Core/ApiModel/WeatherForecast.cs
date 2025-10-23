using NetBuild.Domain.Types;

namespace NetBuild.App.Core.ApiModel
{
    public class WeatherForecast
    {
        public List<DataPoint> Temperature { get; set; } // ºC
        public List<DataPoint> Humidity { get; set; } // %
        public List<DataPoint> DirectSolarRadiation { get; set; } // W/m2

        public WeatherForecast() {
            Temperature = new List<DataPoint>();
            Humidity = new List<DataPoint>();
            DirectSolarRadiation = new List<DataPoint>();
        }
    }
}
