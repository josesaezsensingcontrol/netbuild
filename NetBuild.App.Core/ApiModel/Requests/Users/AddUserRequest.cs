using NetBuild.Domain.Types;

namespace NetBuild.App.Core.ApiModel.Responses.Users
{
    public class AddUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Culture { get; set; }
        public string Password { get; set; }
        public string? ParentId { get; set; } // Only super admin can use this field, ignored otherwise
        public UserRole? Role { get; set; } // Only super admin can use this field, ignored otherwise
    }
}
