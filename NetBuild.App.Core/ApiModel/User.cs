using NetBuild.Domain.Types;

namespace NetBuild.App.Core.ApiModel
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Culture { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string ParentId { get; set; }
        public long? LastLoginDate { get; set; }
        public long CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public long ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
