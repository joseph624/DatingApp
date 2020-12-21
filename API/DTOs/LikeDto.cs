namespace API.DTOs
{
    public class LikeDto
    {
        // create a member card & how to desplay users we like
        public int Id { get; set; } 
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
    }
}