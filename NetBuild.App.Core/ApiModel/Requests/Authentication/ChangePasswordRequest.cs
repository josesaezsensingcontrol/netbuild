namespace NetBuild.App.Core.ApiModel.Responses.Authentication
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
