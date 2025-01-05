using TaskManagement.Domain.Common.HATEOAS;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Common.Pagination;

namespace TaskManagement.Application.Utilities.Pagination
{
    /// <summary>
    /// Represents a paginated list of items.
    /// This class provides properties and methods to facilitate pagination in queries.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PaginatedList<T> : IPaginatedList<T>
    {
        /// <summary>
        /// Gets the collection of items in the current page.
        /// </summary>
        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int Page { get; private set; }

        /// <summary>
        /// Gets the number of items per page.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the total number of items across all pages.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Gets the total number of pages based on the total item count and page size.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets or sets the links related to the paginated list, such as for navigation.
        /// </summary>
        public IList<Link> Links { get; set; } = new List<Link>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items to include in the paginated list.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="totalCount">The total number of items across all pages.</param>
        public PaginatedList(IEnumerable<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize); // Calculate total pages
        }

        /// <summary>
        /// Gets a value indicating whether there is a next page available.
        /// </summary>
        public bool HasNextPage => Page * PageSize < TotalCount;

        /// <summary>
        /// Gets a value indicating whether there is a previous page available.
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Asynchronously creates a new instance of <see cref="PaginatedList{T}"/> based on the provided queryable data.
        /// </summary>
        /// <param name="query">The queryable source of items.</param>
        /// <param name="page">The desired page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="PaginatedList{T}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when page or pageSize is less than 1.</exception>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return Empty(page, pageSize);
            }

            // Ensure pagination arguments are valid
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than zero.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            }

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, page, pageSize, totalCount);
        }

        /// <summary>
        /// Creates an empty paginated list with specified page and page size.
        /// </summary>
        public static PaginatedList<T> Empty(int page, int pageSize)
        {
            return new PaginatedList<T>(Enumerable.Empty<T>(), page, pageSize, 0);
        }
    }
}
