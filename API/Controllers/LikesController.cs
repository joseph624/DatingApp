using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        // give user ability to like another user
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            // get user we are liking
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            // get the source user
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            // check if we have liked user
            if (likedUser == null) return NotFound();
            // make sure liker isn't liking themself
            if (sourceUser.UserName == username) return BadRequest("You Cannot like yourself");

            // check if user like already liked
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already liked this user"); // only giving ability to like a user not unlike

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            // add user like
            sourceUser.LikedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }

        // get the user likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
           likesParams.UserId = User.GetUserId();
           var users =  await _likesRepository.GetUserLikes(likesParams);

           Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

           return Ok(users);
        }
    }
}