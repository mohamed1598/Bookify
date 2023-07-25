using Bookify.WEB.Core.consts;
using Microsoft.AspNetCore.Identity;

namespace Bookify.WEB.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = new()
            {
                UserName = "admin@bookify.com",
                Email = "admin@bookify.com",
                FullName = "admin",
                EmailConfirmed = true,
            };
            var user = await userManager.FindByNameAsync(admin.UserName);
            if (user is null)
            {
                await userManager.CreateAsync(admin,"P@ssword123");
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }
    }
}
