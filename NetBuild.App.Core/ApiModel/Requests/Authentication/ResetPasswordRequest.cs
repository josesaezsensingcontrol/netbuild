namespace NetBuild.App.Core.ApiModel.Responses.Authentication
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Code { get; set; }
    }
}
