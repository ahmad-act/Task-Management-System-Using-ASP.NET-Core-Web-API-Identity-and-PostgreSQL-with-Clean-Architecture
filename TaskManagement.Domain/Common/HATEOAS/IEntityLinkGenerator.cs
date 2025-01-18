using TaskManagement.Domain.Common.Pagination;

namespace TaskManagement.Domain.Common.HATEOAS
{
    public interface IEntityLinkGenerator
    {
        Link GenerateLink(LinkOperation operation, string? id = null, string? searchTerm = null, uint? page = null, uint? pageSize = null, string? sortColumn = null, string? sortOrder = null);
        void PaginationLinks<T>(IPaginatedList<T> list, string? searchTerm, string? sortColumn, string? sortOrder);
        void GenerateHateoasLinks<TKey, T>(T readDto, string? id = null) where T : class, ILinks<TKey>;
    }
}