using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DbContext
{
    public class ApplicationDbSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public ApplicationDbSeeder(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task SeedDatabaseAsync()
        {

        }


    }
}
