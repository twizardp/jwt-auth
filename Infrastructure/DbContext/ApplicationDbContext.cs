using Domain;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext
{
    public class ApplicationDbContext :
        IdentityDbContext<
            User, Role,
            string, IdentityUserClaim<string>,
            IdentityUserRole<string>, IdentityUserLogin<string>,
            RoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        { }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
