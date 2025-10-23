using AutoMapper;
using Entities = NetBuild.Domain.Entities;

namespace NetBuild.App.Core.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<ApiModel.User, Entities.User>()
                .ReverseMap();

            CreateMap<ApiModel.Neighborhood, Entities.Neighborhood>()
                .ReverseMap();

            CreateMap<ApiModel.Building, Entities.Building>()
                .ReverseMap();

            CreateMap<ApiModel.DiagramConfig, Entities.DiagramConfig>()
                .ReverseMap();

            CreateMap<ApiModel.DiagramNode, Entities.DiagramNode>()
                .ReverseMap();

            CreateMap<ApiModel.Schedule, Entities.Schedule>()
                .ReverseMap();

            CreateMap<ApiModel.WeatherForecast, Entities.WeatherForecast>()
                .ReverseMap();

            CreateMap<ApiModel.TimeRange, Entities.TimeRange>()
                .ReverseMap();

            CreateMap<ApiModel.System, Entities.System>()
                .ReverseMap();

            CreateMap<ApiModel.DataInput, Entities.DataInput>()
                .ReverseMap();

            CreateMap<ApiModel.DataInputEntry, Entities.DataInputEntry>()
                .ReverseMap();
        }
    }
}
