using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context, AutoMapper.IMapper _mapper)
        {
            _context = context;

        }

        // get an individual like
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        // a user with their likes & include them in a list of users
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy( u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            // currently logged in user likes
            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser); // users from the likes table 
            }

            //list of users that liked the currently logged in user
            if(likesParams.Predicate == "likedBy" )
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser); // users from the likes table 
            }

            // project to liked dto
            var likedUsers = users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);

        }

        // get likes for a user who they liked & liked by
        // predicate as in liked by or users they like
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            // get user with their colleciton of likes
            // when they add a like we will add it to the user returned by the context
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId );
        }
    }
}