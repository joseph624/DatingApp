using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            // check if there are no users in database
            if (users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            // loop through roles 
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }



            foreach (var user in users)
            {                
                user.UserName = user.UserName.ToLower();
                //takes user we are creating a password
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                    UserName = "admin"
            };
            // create admin user
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            // add the user to the roles
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
        }
    }   
}