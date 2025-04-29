using System.Collections.ObjectModel;

namespace Common.Authorization
{
    public record AppPermission(string Feature, string Action, string Group, string Description, bool IsBasic = false)
    {
        public string Name { get; set; }

        public static string NameFor(string feature, string action) => $"Permissions.{feature}.{action}";

    }

    public class AppPermissions
    {
        private static readonly AppPermission[] _all =
        {
            new(AppFeature.Users,AppAction.Create,AppRoleGroups.SystemAccess,"Create User"),
            new(AppFeature.Users,AppAction.Read,AppRoleGroups.SystemAccess,"View User"),
            new(AppFeature.Users,AppAction.Update,AppRoleGroups.SystemAccess,"Update User"),
            new(AppFeature.Users,AppAction.Delete,AppRoleGroups.SystemAccess,"Delete User"),

           
            new(AppFeature.UserRoles,AppAction.Read,AppRoleGroups.SystemAccess,"View User Roles"),
            new(AppFeature.UserRoles,AppAction.Update,AppRoleGroups.SystemAccess,"Update User Roles"),

            new(AppFeature.Roles,AppAction.Create,AppRoleGroups.SystemAccess,"Create Roles"),
            new(AppFeature.Roles,AppAction.Read,AppRoleGroups.SystemAccess,"View Roles"),
            new(AppFeature.Roles,AppAction.Update,AppRoleGroups.SystemAccess,"Update Roles"),
            new(AppFeature.Roles,AppAction.Delete,AppRoleGroups.SystemAccess,"Delete Roles"),

            new(AppFeature.RoleClaims,AppAction.Read,AppRoleGroups.SystemAccess,"View Role Claims/Permission"),
            new(AppFeature.RoleClaims,AppAction.Update,AppRoleGroups.SystemAccess,"Update Role Claims/Permission"),

            new(AppFeature.Employees,AppAction.Create,AppRoleGroups.Management,"Create Employees"),
            new(AppFeature.Employees,AppAction.Read,AppRoleGroups.Management,"View Employees",true),
            new(AppFeature.Employees,AppAction.Update,AppRoleGroups.Management,"Update Employees"),
            new(AppFeature.Employees,AppAction.Delete,AppRoleGroups.Management,"Delete Employees"),


        };

        public static IReadOnlyList<AppPermission> AdminPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray());

        public static IReadOnlyList<AppPermission> BasicPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());
         
    }
}
