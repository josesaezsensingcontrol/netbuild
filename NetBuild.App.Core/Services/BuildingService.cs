using AutoMapper;
using Microsoft.Extensions.Logging;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.Services.Interface;
using NetBuild.Domain.Repositories;
using NetBuild.Domain.Types;
using Entities = NetBuild.Domain.Entities;

namespace NetBuild.App.Core.Services
{
    internal class BuildingService : IBuildingService
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly ISystemService _systemService;
        private readonly IMapper _mapper;
        private readonly ILogger<BuildingService> _logger;

        public BuildingService(IBuildingRepository buildingRepository, ISystemService systemService, IMapper mapper, ILogger<BuildingService> logger)
        {
            _buildingRepository = buildingRepository;
            _systemService = systemService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CanSeeBuildingAsync(JwtData userData, string neighborhoodId, string buildingId)
        {
            if (userData.Role == UserRole.SuperAdmin || userData.Role == UserRole.Admin)
            {
                return true;
            }
            else 
            {
                return (await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId))?.OwnerId == userData.ParentId;
            }
        }

        public async Task<bool> CanEditBuildingAsync(JwtData userData, string neighborhoodId, string buildingId)
        {
            if (userData.Role == UserRole.SuperAdmin)
            {
                return true;
            }
            else if (userData.Role == UserRole.User)
            {
                return false;
            }
            else 
            {
                return (await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId))?.OwnerId == userData.UserId;
            }
        }

        public async Task<Building> AddBuildingAsync(string neighborhoodId, string ownerId, string name, string description, double latitude, double longitude)
        {
            return _mapper.Map<Building>(await _buildingRepository.AddAsync(new Entities.Building
            {
                Id = Guid.NewGuid().ToString(),
                NeighborhoodId = neighborhoodId,
                OwnerId = ownerId,
                Name = name,
                Description = description ?? "",
                Latitude = latitude,
                Longitude = longitude
            }));
        }

        public async Task<Building?> GetBuildingAsync(string buildingId, string neighborhoodId)
        {
            return _mapper.Map<Building>(await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId));
        }

        public async Task<IEnumerable<Building>> GetAllBuildingsAsync(string? neighborhoodId, string? ownerId = null)
        {
            if (neighborhoodId == null && ownerId == null)
            {
                return (await _buildingRepository.GetAsync()).Select(x => _mapper.Map<Building>(x));
            }
            else if (neighborhoodId == null)
            {
                return (await _buildingRepository.GetAsync(x => x.OwnerId == ownerId)).Select(x => _mapper.Map<Building>(x));
            }
            else if (ownerId == null) 
            {
                return (await _buildingRepository.GetAsync(x => x.NeighborhoodId == neighborhoodId)).Select(x => _mapper.Map<Building>(x));
            }
            else
            {
                return (await _buildingRepository.GetAsync(x => x.NeighborhoodId == neighborhoodId && x.OwnerId == ownerId)).Select(x => _mapper.Map<Building>(x));
            }
        }

        public async Task UpdateBuildingAsync(string buildingId, string neighborhoodId, string name, string description, double latitude, double longitude)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId);
            if (building != null)
            {
                building.Name = name;
                building.Description = description;
                building.Latitude = latitude;
                building.Longitude = longitude;
                await _buildingRepository.UpdateAsync(building);
            }
        }

        public async Task UpdateBuildingScheduleAsync(string buildingId, string neighborhoodId, Schedule schedule)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId);
            if (building != null)
            {
                building.Schedule = _mapper.Map<Entities.Schedule>(schedule);

                await _buildingRepository.UpdateAsync(building);
            }
        }

        public async Task UpdateBuildingDiagramImageAsync(string buildingId, string neighborhoodId, string imageUrl)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId);
            if (building != null)
            {
                if (building.Diagram == null)
                {
                    building.Diagram = new Entities.DiagramConfig();
                }

                building.Diagram.ImageUrl = imageUrl;
                await _buildingRepository.UpdateAsync(building);
            }
        }

        public async Task UpdateBuildingDiagramNodesAsync(string buildingId, string neighborhoodId, IEnumerable<DiagramNode> nodes) 
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId);
            if (building != null)
            {
                if (building.Diagram == null) {
                    building.Diagram = new Entities.DiagramConfig();
                }

                building.Diagram.Nodes = nodes.Select(x => _mapper.Map<Entities.DiagramNode>(x)).ToList();
                await _buildingRepository.UpdateAsync(building);
            }
        }

        public async Task UpdateBuildingWeatherForecastAsync(string buildingId, string neighborhoodId, WeatherForecast newWeatherForecast)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId, neighborhoodId);
            if (building != null)
            {
                building.WeatherForecast = _mapper.Map<Entities.WeatherForecast>(newWeatherForecast);
                await _buildingRepository.UpdateAsync(building);
            }
        }

        public async Task DeleteBuildingAsync(string buildingId, string neighborhoodId)
        {
            foreach (var system in await _systemService.GetAllSystemsAsync(buildingId)) {
                await _systemService.DeleteSystemAsync(system.Id, buildingId);
            }

            await _buildingRepository.DeleteAsync(buildingId, neighborhoodId);
        }
    }
}
