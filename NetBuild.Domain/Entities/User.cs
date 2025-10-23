using NetBuild.Domain.Types;
using System.Net.Mail;

namespace NetBuild.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public UserRole Role { get; set; }
        public string? ParentId { get; set; }
        public string RefreshToken { get; set; }
        public string ResetPasswordCode { get; set; }
        public long? ResetPasswordCodeExpirationDate { get; set; }
        public string Culture { get; set; }
        public long? LastLoginDate { get; set; }
        public string? CreatedBy { get; set; }

        public bool Validate()
        {
            try
            {
                new MailAddress(this.Email);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
