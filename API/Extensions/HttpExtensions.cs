using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        // We don't need to return anything since we add response to header
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            // convert response to camelcase
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // add pagination to response headers 
            // serialize this so it gets key & string value
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            // add cors header
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}