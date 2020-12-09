using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    //Generic - can take any type of entity <T> T could be MemberDto
    // List of users or members...
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize); // if we have total count of 10 and page size is 5 then we get 2 pages from query
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items); // Range of items in constructor so we have access to items in page list
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; } // How Many Items are in this query

        // This recieves the query
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // How many items are from this query
            var count = await source.CountAsync(); // what we get back from database isn't going to equal the total amount available
            // If we are on page 1. Page 1 - 1 = 0. if page size is 5 then we skip no records and take 5
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // Execute query with to list async
            // return new instance of paged list
            return new PagedList<T>(items, count, pageNumber, pageSize);
        } 
    }
}