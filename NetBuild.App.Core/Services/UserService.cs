using AutoMapper;
using Microsoft.Extensions.Logging;
using NetBuild.App.Core.Crypto;
using NetBuild.App.Core.Services.Interfaces;
using NetBuild.Domain.Repositories;
using NetBuild.Domain.Types;
using Entities = NetBuild.Domain.Entities;

namespace NetBuild.App.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> AddAsync(ApiModel.User user)
        {
            var entity = _mapper.Map<Entities.User>(user);
            entity.Password = HashHelper.HashPassword(user.Password, out var salt);
            entity.PasswordSalt = salt;

            return await _userRepository.AddAsync(entity);
        }

        public async Task DeleteAsync(string userId)
        {
            await _userRepository.DeleteAsync(userId);
        }

        public async Task<bool> ExistsUserAsync(string email)
        {
            var entity = await GetUserByEmailAsync(email);
            return entity != null;
        }

        public async Task<IEnumerable<ApiModel.User>> GetAllAsync(string? parentId, UserRole? role)
        {
            IEnumerable<Entities.User> entities;
            if (parentId == null) {
                // Only super admin will be requesting role filtered users
                entities = await _userRepository.GetAsync(role != null ? x => x.Role == role : null);
            } else {
                entities = await _userRepository.GetAsync(x => x.ParentId == parentId);
            }

            var users = _mapper.Map<List<ApiModel.User>>(entities.ToList());
            foreach (var user in users)
            {
                user.Password = string.Empty;
            }

            return users;
        }

        public async Task<ApiModel.User?> GetUserByEmailAsync(string email)
        {
            var userEntity = await _userRepository.GetUserByEmailAsync(email);

            if (userEntity != null) {
                userEntity.Password = string.Empty;
            }

            return _mapper.Map<ApiModel.User>(userEntity);
        }

        public async Task<ApiModel.User?> GetUserByIdAsync(string userId)
        {
            var userEntity = await _userRepository.GetByIdAsync(userId);
            
            if (userEntity != null)
            {
                userEntity.Password = string.Empty;
            }

            return _mapper.Map<ApiModel.User>(userEntity);
        }

        public async Task UpdateAsync(ApiModel.User user)
        {
            var userEntity = await _userRepository.GetByIdAsync(user.Id);

            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.Email = user.Email;
            userEntity.Culture = user.Culture;

            if (!string.IsNullOrEmpty(user.Password))
            {
                userEntity.Password = HashHelper.HashPassword(user.Password, out var salt);
                userEntity.PasswordSalt = salt;
            }

            await _userRepository.UpdateAsync(userEntity);
        }
    }
}
