using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext
{
    public class ApplicationDbContext :
        IdentityDbContext<
            IdentityUser, IdentityRole,
            string, IdentityUserClaim<string>,
            IdentityUserRole<string>, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        { }

        public DbSet<Employee> Employees => Set<Employee>();
    }
}
