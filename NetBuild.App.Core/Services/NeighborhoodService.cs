using AutoMapper;
using Microsoft.Extensions.Logging;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.Services.Interface;
using NetBuild.Domain.Repositories;
using Entities = NetBuild.Domain.Entities;

namespace NetBuild.App.Core.Services
{
    public class NeighborhoodService : INeighborhoodService
    {
        private readonly INeighborhoodRepository _neighborhoodRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NeighborhoodService> _logger;

        public NeighborhoodService(INeighborhoodRepository neighborhoodRepository, ISystemService systemService, IMapper mapper, ILogger<NeighborhoodService> logger)
        {
            _neighborhoodRepository = neighborhoodRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Neighborhood?> AddNeighborhoodAsync(string name, string description)
        {
            return _mapper.Map<Neighborhood>(await _neighborhoodRepository.AddAsync(new Entities.Neighborhood
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description ?? "",
            }));
        }

        public async Task<Neighborhood?> GetNeighborhoodAsync(string id)
        {
            return _mapper.Map<Neighborhood>(await _neighborhoodRepository.GetByIdAsync(id));
        }

        public async Task<IEnumerable<Neighborhood>> GetAllNeighborhoodsAsync()
        {
            return (await _neighborhoodRepository.GetAsync()).Select(x => _mapper.Map<Neighborhood>(x));
        }

        public async Task UpdateNeighborhoodAsync(string id, string name, string description)
        {
            var neighborhood = await _neighborhoodRepository.GetByIdAsync(id);
            if (neighborhood != null)
            {
                neighborhood.Name = name;
                neighborhood.Description = description;
                await _neighborhoodRepository.UpdateAsync(neighborhood);
            }
        }

        public async Task DeleteNeighborhoodAsync(string id)
        {
            await _neighborhoodRepository.DeleteAsync(id);
        }
    }
}
