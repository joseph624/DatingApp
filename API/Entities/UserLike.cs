namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } // user that likes the other user
        public int SourceUserId { get; set; }

        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}