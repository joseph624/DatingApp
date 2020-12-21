namespace API.Helpers
{
    public class PaginationParams
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

    }
}