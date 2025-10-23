namespace NetBuild.App.Core.Configuration
{
    public class AuthenticationConfiguration
    {
        public string SigningKey { get; set; }
        public string DefaultIssuer { get; set; }
        public string DefaultAudience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int ResetPasswordCodeExpirationMinutes { get; set; }
    }
}
