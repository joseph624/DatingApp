namespace API.Helpers
{
    public class UserParams
    {
        // Set Maximum Page - Most amount items per request
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;        
        private int _pageSize = 10; // Default page size

        public int PageSize
        {
            get => _pageSize;
            // if page size is grater then make page size
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public string OrderBy { get; set; } = "lastActive";
        
    }
}