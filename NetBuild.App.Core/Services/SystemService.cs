using AutoMapper;
using Microsoft.Extensions.Logging;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.ApiModel.SignalR;
using NetBuild.App.Core.Services.Interface;
using NetBuild.Domain.Repositories;
using NetBuild.Domain.Types;
using Entities = NetBuild.Domain.Entities;

namespace NetBuild.App.Core.Services
{
    public class SystemService : ISystemService
    {
        private readonly IDataService _dataService;
        private readonly ISignalRService _signalRService;
        private readonly ISystemRepository _systemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemService> _logger;

        public SystemService(IDataService dataService, ISignalRService signalRService, ISystemRepository systemRepository, IMapper mapper, ILogger<SystemService> logger)
        {
            _dataService = dataService;
            _signalRService = signalRService;
            _systemRepository = systemRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiModel.System?> AddSystemAsync(string systemId, string buildingId, string name, IEnumerable<DataInput> dataInputs, IDictionary<string, string> metadata)
        {
            return _mapper.Map<ApiModel.System>((await _systemRepository.AddAsync(new Entities.System { 
                    Id = systemId,
                    BuildingId = buildingId,
                    Name = name,
                    DataInputs = _mapper.Map<IEnumerable<Entities.DataInput>>(dataInputs),
                    Metadata = metadata
                }))
            );
        }

        public async Task<IEnumerable<ApiModel.System>> GetAllSystemsAsync(string? buildingId = null)
        {
            if (buildingId == null)
            {
                return (await _systemRepository.GetAsync()).Select(x => _mapper.Map<ApiModel.System>(x));
            }
            else
            {
                return (await _systemRepository.GetAsync(x => x.BuildingId == buildingId)).Select(x => _mapper.Map<ApiModel.System>(x));
            }
        }

        public async Task<ApiModel.System?> GetSystemAsync(string systemId, string? buildingId = null)
        {
            Entities.System system;

            if (buildingId == null)
            {
                system = (await _systemRepository.GetAsync(x => x.Id == systemId)).SingleOrDefault();
            }
            else
            {
                system = (await _systemRepository.GetByIdAsync(systemId, buildingId));
            }

            return system != null ? _mapper.Map<ApiModel.System>(system) : null;
        }

        public async Task UpdateSystemAsync(string systemId, string buildingId, string name)
        {
            var system = await _systemRepository.GetByIdAsync(systemId, buildingId);
            
            if (system != null)
            {
                system.Name = name;
                await _systemRepository.UpdateAsync(system);
            }
        }

        public async Task UpdateSystemValuesAsync(string systemId, string buildingId, IDictionary<string, DataPoint> newValues)
        {
            var system = await _systemRepository.GetByIdAsync(systemId, buildingId);

            if (system != null)
            {
                var updatedValues = new Dictionary<string, TimestampedValue>();
                foreach(var input in system.DataInputs)
                {
                    if (newValues.ContainsKey(input.Id) && !double.IsNaN(newValues[input.Id].Value) && !double.IsInfinity(newValues[input.Id].Value))
                    {
                        if (!input.Date.HasValue || input.Date.Value <= newValues[input.Id].Timestamp) {
                            input.Date = newValues[input.Id].Timestamp;
                            input.Value = newValues[input.Id].Value;
                            updatedValues[input.Id] = new TimestampedValue { Date = input.Date.Value, Value = input.Value.Value };
                        }

                        await _dataService.AddHistoricDataAsync(buildingId, systemId, input.Id, input.Value.Value, input.Date.Value);
                    }
                }

                if (updatedValues.Count > 0)
                {
                    await _systemRepository.UpdateAsync(system);

                    try
                    {
                        await _signalRService.SendAsync(buildingId, "newSystemData", new NewDataMessage
                        {
                            BuildingId = buildingId,
                            SystemId = systemId,
                            DataInputs = updatedValues
                        });
                    }
                    catch (Exception) { }
                }
            }
        }

        public async Task UpdateSystemBulkValuesAsync(string systemId, string buildingId, IDictionary<string, List<DataPoint>> newValues)
        {
            var system = await _systemRepository.GetByIdAsync(systemId, buildingId);

            if (system != null)
            {
                var updatedValues = new Dictionary<string, TimestampedValue>();
                foreach (var input in system.DataInputs)
                {
                    if (newValues.ContainsKey(input.Id) && newValues[input.Id].Count > 0)
                    {
                        var validValues = newValues[input.Id]
                            .Where(x => !double.IsNaN(x.Value) && !double.IsInfinity(x.Value))
                            .ToList();

                        if (validValues.Count > 0)
                        {
                            validValues.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

                            if (!input.Date.HasValue || input.Date.Value <= validValues.First().Timestamp)
                            {
                                input.Date = validValues.First().Timestamp;
                                input.Value = validValues.First().Value;
                                updatedValues[input.Id] = new TimestampedValue { Date = input.Date.Value, Value = input.Value.Value };
                            }

                            await _dataService.AddHistoricBulkDataAsync(buildingId, systemId, input.Id, validValues);
                        }
                    }
                }

                if (updatedValues.Count > 0)
                {
                    await _systemRepository.UpdateAsync(system);

                    try
                    {
                        await _signalRService.SendAsync(buildingId, "newSystemData", new NewDataMessage
                        {
                            BuildingId = buildingId,
                            SystemId = systemId,
                            DataInputs = updatedValues
                        });
                    } catch (Exception) { }
                }
            }
        }

        public async Task AddSystemPredictionsAsync(string systemId, string buildingId, IDictionary<string, DataPoint> newValues)
        {
            var system = await _systemRepository.GetByIdAsync(systemId, buildingId);

            if (system != null)
            {
                foreach (var input in system.DataInputs)
                {
                    if (newValues.ContainsKey(input.Id) && !double.IsNaN(newValues[input.Id].Value) && !double.IsInfinity(newValues[input.Id].Value))
                    {
                        await _dataService.AddPredictionDataAsync(buildingId, systemId, input.Id, newValues[input.Id].Value, newValues[input.Id].Timestamp);
                    }
                }
            }
        }

        public async Task AddSystemBulkPredictionsAsync(string systemId, string buildingId, IDictionary<string, List<DataPoint>> newValues)
        {
            var system = await _systemRepository.GetByIdAsync(systemId, buildingId);

            if (system != null)
            {
                foreach (var input in system.DataInputs)
                {
                    if (newValues.ContainsKey(input.Id) && newValues[input.Id].Count > 0)
                    {
                        await _dataService.AddPredictionBulkDataAsync(buildingId, systemId, input.Id, newValues[input.Id].Where(x => !double.IsNaN(x.Value) && !double.IsInfinity(x.Value)).ToList());
                    }
                }
            }
        }

        public async Task DeleteSystemAsync(string systemId, string buildingId)
        {
            var system = await _systemRepository.GetByIdAsync(systemId, buildingId);

            if (system != null) {
                bool success = true;
                
                foreach (var input in system.DataInputs) {
                    if (!success) {
                        break;
                    }
                    success = await _dataService.DeleteAllHistoricDataAsync($"{buildingId}-{systemId}-{input.Id}") && await _dataService.DeleteAllPredictionDataAsync($"{buildingId}-{systemId}-{input.Id}");
                }

                if (success)
                {
                    await _systemRepository.DeleteAsync(systemId, buildingId);
                }
            }
        }
    }
}
