using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public string Description { get; set; }
        public string Group { get; set; }
    }
}
