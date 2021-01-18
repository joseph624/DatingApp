using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public  async Task<ActionResult> GetUsersWithRoles()
        {
            // get object with user id, username & roles they are in and send to list
            var users = await _userManager.Users
                .Include(r => r.UserRoles) // get user role list of roles
                .ThenInclude(r => r.Role) // then include the role
                .OrderBy(u => u.UserName)
                .Select(u => new 
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }


        // Edit Roles for Admin
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();
            // get the user from the parameter
            var user = await _userManager.FindByNameAsync(username);

            // check if we have user
            if  (user == null) return NotFound("Could not find user");

            // get the list of roles for the user
            var userRoles = await _userManager.GetRolesAsync(user);

            // look at list of roles and add user to role unless they are in that role
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            //check to see if add succeeded
            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            // remove user from roles that they were currently in
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            // check if roles removed
            if (!result.Succeeded) BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));


        }



        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or Moderators can see this");
        }
    }
}