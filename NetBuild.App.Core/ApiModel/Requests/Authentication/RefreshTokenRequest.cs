namespace NetBuild.App.Core.ApiModel.Responses.Authentication
{
    public class RefreshTokenRequest
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
