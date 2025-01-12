using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Domain.Common.Pagination
{
    public interface IPaginatedList<T>
    {
        IEnumerable<T> Items { get; }
        uint Page { get; }
        uint PageSize { get; }
        uint TotalCount { get; }
        uint TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }

        IList<Link> Links { get; set; }
    }
}