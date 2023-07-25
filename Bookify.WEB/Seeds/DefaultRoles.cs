using Bookify.WEB.Core.consts;
using Microsoft.AspNetCore.Identity;

namespace Bookify.WEB.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if(!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Archive));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Reception));
            }
        }
    }
}
