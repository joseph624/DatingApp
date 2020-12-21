using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        // get an individual like
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

        // get a user with their likes & include them
        Task<AppUser> GetUserWithLikes(int userId);

        // get likes for a user who they liked & liked by
        // predicate as in liked by or users they like
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}