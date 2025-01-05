using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Domain.Common.Pagination
{
    public interface IPaginatedList<T>
    {
        IEnumerable<T> Items { get; }
        int Page { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }

        IList<Link> Links { get; set; }
    }
}