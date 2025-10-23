using AutoMapper;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.ApiModel.Responses;
using NetBuild.App.Core.Configuration;
using NetBuild.App.Core.Constants;
using NetBuild.App.Core.Crypto;
using NetBuild.App.Core.Localization;
using NetBuild.App.Core.Services.Interface;
using NetBuild.App.Core.Services.Interfaces;
using NetBuild.Domain.Repositories;
using NetBuild.Domain.Types;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entities = NetBuild.Domain.Entities;

namespace NetBuild.App.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
        private const string Bearer = "Bearer";

        private readonly SymmetricSecurityKey Key;
        private readonly Random Rng = new Random();

        private readonly AuthenticationConfiguration _authConfig;
        private readonly ClientAuthConfiguration _clientAuthConfig;

        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;

        private readonly IMapper _mapper;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IUserRepository userRepository, IMailService mailService, IOptions<AuthenticationConfiguration> authConfig, IOptions<ClientAuthConfiguration> clientAuthConfig, IMapper mapper, ILogger<AuthenticationService> logger) {
            _userRepository = userRepository;
            _mailService = mailService;

            _authConfig = authConfig.Value;
            _clientAuthConfig = clientAuthConfig.Value;

            _mapper = mapper;
            _logger = logger;

            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfig.SigningKey));
        }
        public async Task<TokensData?> LoginAsync(string email, string password) {
            _logger.LogInformation($"Login -> Trying to authenticate user {email}");

            var user = await _userRepository.GetUserByEmailAsync(email.ToLowerInvariant());

            if (user != null && ValidateUserCredentials(password, user.PasswordSalt, user.Password))
            {
                _logger.LogInformation($"Login -> Credentials validated for user {user.Id}");

                var tokens = GenerateTokens(user.Id.ToString(), user.Email, user.FirstName, user.LastName, user.Role, user.ParentId);

                try
                {
                    _logger.LogInformation($"Login -> Generated JWT Token and Refresh Token for user {user.Id}, updating last login date and refresh token in DB");

                    user.RefreshToken = tokens.RefreshToken;
                    user.LastLoginDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    await _userRepository.UpdateAsync(user);

                    return tokens;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Login -> Error updating last login date for{user.Id}");
                    return null;
                }
            }

            return null;
        }

        public async Task<TokensData?> RefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user!= null && user.RefreshToken == refreshToken)
            {
                var tokens = GenerateTokens(user.Id.ToString(), user.Email, user.FirstName, user.LastName, user.Role, user.ParentId);

                user.RefreshToken = tokens.RefreshToken;

                _logger.LogInformation($"Refresh Token -> Generated JWT Token and Refresh Token for user {user.Id}, updating last login date and refresh token in DB");

                try
                {
                    await _userRepository.UpdateAsync(user);
                    return tokens;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Login -> Error updating last login date for{user.Id}");
                }
            }

            return null;
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                _logger.LogInformation($"Logout -> {userId}, updating refresh token in DB");

                user.RefreshToken = null;

                try
                {
                    await _userRepository.UpdateAsync(user);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Logout -> Error updating last login date for{user.Id}");
                }

                return false;
            }

            return true;
        }

        public async Task<string?> RegisterUserAsync(string email, string password, string firstName, string lastName, string culture) {
            var newUser = new Entities.User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Email = email.ToLowerInvariant(),
                Password = HashHelper.HashPassword(password, out string salt),
                PasswordSalt = salt,
                Culture = culture
            };

            _logger.LogInformation($"Register User -> Id {newUser.Email}");

            if (newUser.Validate())
            {
                _logger.LogInformation($"Register User -> Generating tokens for the new user {newUser.Id}");

                try
                {
                    var newUserId = await _userRepository.AddAsync(newUser);
                    _logger.LogInformation($"Register User -> New user with Id {newUser.Email} registered successfully");
                    
                    return newUserId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Register User -> Error registering new user with Id {newUser.Email}");
                }
            }

            _logger.LogError($"Register User -> Error registering new user with Id {newUser.Email}");
            return null;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword) {
            var user = await _userRepository.GetByIdAsync(userId);

            _logger.LogInformation($"Change Password -> Validating old credentials for user {userId}");
            if (user != null && ValidateUserCredentials(oldPassword, user.PasswordSalt, user.Password))
            {
                _logger.LogInformation($"Change Password -> Updating password for user {user.Id}");
                user.Password = HashHelper.HashPassword(newPassword, out string salt);
                user.PasswordSalt = salt;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation($"Change Password -> Update password successful for user {user.Id}");
                return true;
            }

            return false;
        }

        public async Task<bool> ResetPasswordCodeRequestAsync(string userId) {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null && (string.IsNullOrEmpty(user.ResetPasswordCode) || !user.ResetPasswordCodeExpirationDate.HasValue || user.ResetPasswordCodeExpirationDate <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()))
            {
                _logger.LogInformation($"Reset Password Code Request -> Generating new code for user {user.Id}");
                user.ResetPasswordCode = Rng.Next(0, 999999).ToString("D6");
                user.ResetPasswordCodeExpirationDate = DateTimeOffset.UtcNow.AddMinutes(_authConfig.ResetPasswordCodeExpirationMinutes).ToUnixTimeMilliseconds();
                
                try
                {
                    await _userRepository.UpdateAsync(user);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Reset Password Code Request -> Error updating database for user {user.Id}");
                    return false;
                }
            }
            else
            {
                _logger.LogInformation($"Reset Password Code Request -> Sending still valid code for user {user.Id}");
            }

            try
            {
                var subject = EmailTemplates.ResourceManager.GetString(nameof(EmailTemplates.RESET_PASSWORD_SUBJECT), CultureInfo.GetCultureInfo(user.Culture));
                var body = EmailTemplates.ResourceManager.GetString(nameof(EmailTemplates.RESET_PASSWORD_BODY_TEMPLATE), CultureInfo.GetCultureInfo(user.Culture));
                
                if (await _mailService.SendMailAsync(user.Email, subject, null, body, new object[] { user.ResetPasswordCode }))
                {
                    _logger.LogInformation($"Reset Password Code Request -> Reset password code request completed successfully for user {user.Id}");
                    return true;
                }

                _logger.LogError($"Reset Password Code Request -> Error sending email for user {user.Id}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Reset Password Code Request -> Error sending email for user {user.Id}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string userId, string code, string newPassword) {
            var user = await _userRepository.GetByIdAsync(userId);

            if (!string.IsNullOrEmpty(user.ResetPasswordCode) &&
                user.ResetPasswordCode == code &&
                user.ResetPasswordCodeExpirationDate.HasValue &&
                user.ResetPasswordCodeExpirationDate.Value >= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            {
                _logger.LogInformation($"Reset Password -> Resetting password for user {user.Id}");

                user.Password = HashHelper.HashPassword(newPassword, out string salt);
                user.PasswordSalt = salt;
                user.ResetPasswordCode = null;
                user.ResetPasswordCodeExpirationDate = null;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation($"Reset Password -> Update password successful for user {user.Id}");
                return true;
            }

            return false;
        }

        public JwtSecurityToken CreateJwtSecurityToken(string userId, string email, string firstName, string lastName, UserRole role, string? parentId)
        {
            var subject = new ClaimsIdentity(new[]
            {
                new Claim(AuthConstants.SidClaim, userId, ClaimValueTypes.String, _authConfig.DefaultIssuer),
                new Claim(ClaimTypes.Email, email, ClaimValueTypes.String, _authConfig.DefaultIssuer),
                new Claim(ClaimTypes.GivenName, firstName, ClaimValueTypes.String, _authConfig.DefaultIssuer),
                new Claim(ClaimTypes.Surname, lastName, ClaimValueTypes.String, _authConfig.DefaultIssuer),
                new Claim(ClaimTypes.Role, role.ToString("D"), ClaimValueTypes.String, _authConfig.DefaultIssuer)
            });

            if (parentId != null) {
                subject.AddClaim(new Claim(AuthConstants.ParentClaim, parentId, ClaimValueTypes.String, _authConfig.DefaultIssuer));
            }

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _authConfig.DefaultAudience,
                Subject = subject,
                IssuedAt = DateTime.UtcNow,
                Issuer = _authConfig.DefaultIssuer,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_authConfig.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256),
            };

            return TokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        }

        public void ValidateApiKey(HttpRequestData req)
        {
            _logger.LogInformation("Validating Auth with Api Key");

            if (!req.Headers.TryGetValues(_clientAuthConfig.ApiKeyHeaderName, out var reqToken) ||
                string.IsNullOrEmpty(reqToken.FirstOrDefault()) ||
                _clientAuthConfig.ApiKey != reqToken.First().ToString()
            )
            {
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<IList<Claim>> ValidateAuthJWTAsync(HttpRequestData req, string headerName) {
            _logger.LogInformation("Validate Auth JWT -> Validating custom credentials");

            if (!req.Headers.TryGetValues(headerName, out var reqToken) || string.IsNullOrEmpty(reqToken.FirstOrDefault()))
            {
                throw new UnauthorizedAccessException();
            }

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidAudience = _authConfig.DefaultAudience,
                IssuerSigningKey = Key,
                ValidIssuer = _authConfig.DefaultIssuer,
                RequireExpirationTime = true,
                ValidateLifetime = true,
            };

            try
            {
                var tokenString = reqToken.First().ToString().Contains(Bearer) ? reqToken.First().ToString().Split(' ')[1] : reqToken.First().ToString();
                var tokenValidationResult = await TokenHandler.ValidateTokenAsync(tokenString, validationParameters);

                if (tokenValidationResult.IsValid)
                {
                    return tokenValidationResult.ClaimsIdentity.Claims.ToList();
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Validate Auth JWT -> Not able to validate custom credentials {e}");
                throw;
            }
        }

        public async Task<JwtData> ValidateAuthJWTAsyncAndGetData(HttpRequestData req, string headerName)
        {
            return new JwtData(await ValidateAuthJWTAsync(req, headerName));
        }

        public async Task<User?> ValidateAuthJWTAndGetUserAsync(HttpRequestData req, string headerName)
        {
            var jwtData = await ValidateAuthJWTAsyncAndGetData(req, headerName);

            if (jwtData.UserId == Guid.Empty.ToString()) 
            {
                return null;
            }

            return _mapper.Map<User>(await _userRepository.GetByIdAsync(jwtData.UserId));
        }

        public bool ValidateUserCredentials(string password, string passwordSalt, string hashedPassword)
        {
            return HashHelper.ValidateHashedPassword(password, passwordSalt, hashedPassword);
        }

        private TokensData GenerateTokens(string userId, string email, string firstName, string lastName, UserRole role, string parentId)
        {
            var jwtAccessToken = CreateJwtSecurityToken(userId, email, firstName, lastName, role, parentId);

            // Refresh token (they do not expire, they are single use, refreshed every time an access token is requested)
            var refreshToken = Guid.NewGuid().ToString("N");

            return new TokensData(jwtAccessToken.RawData, ((DateTimeOffset)jwtAccessToken.ValidTo.ToUniversalTime()).ToUnixTimeMilliseconds(), refreshToken);
        }
    }
}
