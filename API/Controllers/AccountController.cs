using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly SignInManager<AppUser> _signInManager;

        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        /// api/account/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Check if username is taken
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            // Get all properties from Register DTO and map to App User object
            var user = _mapper.Map<AppUser>(registerDto);

            //Put Username lowercase
            user.UserName = registerDto.Username.ToLower();

            // creates a user & saves changes into the database
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // check to see if create succeeded
            if (!result.Succeeded) return BadRequest(result.Errors);

            // add the user into the member role
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            // check to see if role result is successful
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        // api/account/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // use user manager to access users table
            var user = await _userManager.Users
                .Include(p => p.Photos)  // bring back photos for the user
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());  // get the user and covert username to lowercase

            if (user == null) return Unauthorized("Invalid username"); // if user is not found then error

            // use sign in manger to access the user
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false); // false means we will not lock out user if password is wrong

            // check to see if result succeeded
            if(!result.Succeeded) return Unauthorized();



            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }


        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }




    }
}