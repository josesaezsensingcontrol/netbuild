using NetBuild.App.Core.Constants;
using NetBuild.Domain.Types;
using System.Security.Claims;

namespace NetBuild.App.Core.ApiModel
{
    public class JwtData
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public string? ParentId { get; set; }

        public JwtData(IEnumerable<Claim> claims)
        {
            UserId = claims.Single(x => x.Type == AuthConstants.SidClaim).Value;
            Email = claims.Single(x => x.Type == ClaimTypes.Email).Value;
            FirstName = claims.Single(x => x.Type == ClaimTypes.GivenName).Value;
            LastName = claims.Single(x => x.Type == ClaimTypes.Surname).Value;
            Role = Enum.Parse<UserRole>(claims.Single(x => x.Type == ClaimTypes.Role).Value);
            ParentId = claims.SingleOrDefault(x => x.Type == AuthConstants.ParentClaim)?.Value;
        }
    }
}
