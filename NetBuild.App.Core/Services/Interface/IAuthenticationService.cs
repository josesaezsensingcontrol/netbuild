using Microsoft.Azure.Functions.Worker.Http;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.ApiModel.Responses;
using System.Security.Claims;

namespace NetBuild.App.Core.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokensData?> LoginAsync(string email, string password);

        Task<TokensData?> RefreshTokenAsync(string userId, string refreshToken);

        Task<bool> LogoutAsync(string userId);

        Task<string?> RegisterUserAsync(string email, string password, string firstName, string lastName, string culture);

        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);

        Task<bool> ResetPasswordCodeRequestAsync(string userId);

        Task<bool> ResetPasswordAsync(string userId, string code, string newPassword);

        bool ValidateUserCredentials(string password, string passwordSalt, string hashedPassword);

        Task<IList<Claim>> ValidateAuthJWTAsync(HttpRequestData req, string headerName);

        Task<JwtData> ValidateAuthJWTAsyncAndGetData(HttpRequestData req, string headerName);

        Task<User?> ValidateAuthJWTAndGetUserAsync(HttpRequestData req, string headerName);

        void ValidateApiKey(HttpRequestData req);
    }
}
