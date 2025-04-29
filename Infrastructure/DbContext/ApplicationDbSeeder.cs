using Common.Authorization;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// 初始化数据种子，基础角色和管理用户
        /// </summary>
        public async Task SeedDatabaseAsync()
        {
            await CheckAndApplyPendingMigrationsAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task CheckAndApplyPendingMigrationsAsync()
        {
            if ((await _dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                await _dbContext.Database.MigrateAsync();
            }
        }

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in AppRoles.DefaultRoles)
            {
                if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName)
                    is not Role role)
                {
                    role = new Role
                    {
                        Name = roleName,
                        Description = roleName + " Role."
                    };
                    await _roleManager.CreateAsync(role);
                }

                // Assign permissions
                if (roleName == AppRoles.Admin)
                {
                    await AssignPermissionsToRolesAsync(role, AppPermissions.AdminPermissions);
                }
                else if (roleName == AppRoles.Basic)
                {
                    await AssignPermissionsToRolesAsync(role, AppPermissions.BasicPermissions);
                }
            }
        }

        private async Task AssignPermissionsToRolesAsync(Role role, IReadOnlyList<AppPermission> permissions)
        {
            var currentClaim = await _roleManager.GetClaimsAsync(role);
            foreach (var permission in permissions)
            {
                if (!currentClaim.Any(c => c.Type == AppClaim.Permission && c.Value == permission.Name))
                {
                    await _dbContext.RoleClaims.AddAsync(new RoleClaim()
                    {
                        RoleId = role.Id,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group
                    });
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminUsername = AppCredentials.Email[..AppCredentials.Email.IndexOf('@')].ToLowerInvariant();
            var adminUser = new User
            {
                UserName = adminUsername,
                Name = "admin",
                Email = AppCredentials.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
                NormalizedUserName = adminUsername.ToUpperInvariant(),
                IsActive = true
            };

            if (!await _userManager.Users.AnyAsync(u => u.Email == AppCredentials.Email))
            {
                var password = new PasswordHasher<User>();
                adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.Password);
                await _userManager.CreateAsync(adminUser);
            }

            if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic))
            {
                await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
            }
        }
    }
}