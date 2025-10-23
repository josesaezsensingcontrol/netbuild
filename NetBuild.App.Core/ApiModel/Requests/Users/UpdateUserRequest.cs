namespace NetBuild.App.Core.ApiModel.Responses.Users
{
    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Culture { get; set; }
        public string? Password { get; set; }
    }
}
