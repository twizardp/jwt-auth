using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
