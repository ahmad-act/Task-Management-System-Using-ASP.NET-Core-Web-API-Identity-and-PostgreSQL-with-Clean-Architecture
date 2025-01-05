using TaskManagement.Domain.Common.HATEOAS;
using Microsoft.AspNetCore.Http;
using TaskManagement.Application.DTOs.BaseDTOs;
using TaskManagement.Domain.Common.Pagination;

namespace TaskManagement.Application.Utilities.HATEOAS
{
    /// <summary>
    /// Provides methods for generating HATEOAS links for various entity operations.
    /// This class constructs URIs based on the current HTTP context, allowing for standardized link generation.
    /// </summary>
    public class EntityLinkGenerator : IEntityLinkGenerator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLinkGenerator"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        public EntityLinkGenerator(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Generates a HATEOAS link based on the specified operation type and optional parameters for entity ID, search, pagination, and sorting.
        /// </summary>
        /// <param name="operation">The operation type (e.g., Get, Create, Update, Delete, List, etc.) to generate a link for.</param>
        /// <param name="id">Optional unique identifier for a specific entity.</param>
        /// <param name="searchTerm">Optional search term for filtering results.</param>
        /// <param name="page">Optional page number for pagination.</param>
        /// <param name="pageSize">Optional number of items per page for pagination.</param>
        /// <param name="sortColumn">Optional column name for sorting results.</param>
        /// <param name="sortOrder">Optional order direction (e.g., ascending or descending) for sorting results.</param>
        /// <returns>
        /// A <see cref="Link"/> instance representing the generated link based on the current HTTP context and specified parameters.
        /// Returns <c>null</c> if the HTTP context is unavailable.
        /// </returns>
        public Link GenerateLink(LinkOperation operation,
                                 string? id = null,
                                 string? searchTerm = null,
                                 int? page = null,
                                 int? pageSize = null,
                                 string? sortColumn = null,
                                 string? sortOrder = null)
        {
            if (_httpContextAccessor?.HttpContext is null)
            {
                return null;
            }

            HttpRequest request = _httpContextAccessor.HttpContext.Request;
            string uri = $"{request.Scheme}://{request.Host}{request.Path}";
            var uriBuilder = new UriBuilder(uri);

            // Add query parameters for list or pagination operations if no specific ID is provided
            if (string.IsNullOrWhiteSpace(id))
            {
                if (operation is LinkOperation.List or LinkOperation.First or LinkOperation.Last or LinkOperation.Previous or LinkOperation.Next or LinkOperation.OtherPages)
                {
                    var query = new QueryString()
                        .Add("searchTerm", searchTerm)
                        .Add("page", page?.ToString())
                        .Add("pageSize", pageSize?.ToString())
                        .Add("sortColumn", sortColumn)
                        .Add("sortOrder", sortOrder);

                    uriBuilder.Query = query.ToString();
                }
            }
            else
            {
                uri = uri.EndsWith(id) ? uri : $"{uri}/{id}";
                uriBuilder = new UriBuilder(uri);
            }

            // Set the appropriate HTTP method for the link based on the operation
            var method = operation switch
            {
                LinkOperation.List => LinkMethod.GET,
                LinkOperation.Get => LinkMethod.GET,
                LinkOperation.Create => LinkMethod.POST,
                LinkOperation.Update => LinkMethod.PUT,
                LinkOperation.Delete => LinkMethod.DELETE,
                LinkOperation.First => LinkMethod.GET,
                LinkOperation.Last => LinkMethod.GET,
                LinkOperation.Previous => LinkMethod.GET,
                LinkOperation.Next => LinkMethod.GET,
                _ => LinkMethod.GET
            };

            return new Link
            {
                Method = method.ToString(),
                Href = uriBuilder.ToString(),
                Operation = operation.ToString()
            };
        }

        /// <summary>
        /// Adds pagination links to a paginated list, including links to the first, previous, next, last pages, and a range of surrounding pages.
        /// </summary>
        /// <typeparam name="T">The type of items in the paginated list.</typeparam>
        /// <param name="list">The paginated list of items to which links will be added.</param>
        /// <param name="searchTerm">Optional search term used for filtering results.</param>
        /// <param name="pageSize">Optional page size for pagination.</param>
        /// <param name="sortColumn">Optional column for sorting the results.</param>
        /// <param name="sortOrder">Optional sorting order (e.g., ascending or descending).</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="totalPages">The total number of available pages.</param>
        public void PaginationLinks<T>(IPaginatedList<T> list,
                                       string? searchTerm,
                                       string? sortColumn,
                                       string? sortOrder)
        {
            // Add links to the first and previous pages if not on the first page
            if (list.Page > 1)
            {
                list.Links.Add(GenerateLink(LinkOperation.First, string.Empty, searchTerm, 1, list.PageSize, sortColumn, sortOrder));
                list.Links.Add(GenerateLink(LinkOperation.Previous, string.Empty, searchTerm, list.Page - 1, list.PageSize, sortColumn, sortOrder));
            }

            // Add links to the next and last pages if not on the last page
            if (list.Page < list.TotalPages)
            {
                list.Links.Add(GenerateLink(LinkOperation.Next, string.Empty, searchTerm, list.Page + 1, list.PageSize, sortColumn, sortOrder));
                list.Links.Add(GenerateLink(LinkOperation.Last, string.Empty, searchTerm, list.TotalPages, list.PageSize, sortColumn, sortOrder));
            }

            // Add links to surrounding pages (up to 3 before and after the current page)
            for (int i = Math.Max(1, list.Page - 3); i <= Math.Min(list.TotalPages, list.Page + 3); i++)
            {
                if (i != list.Page) // Exclude current page link
                {
                    list.Links.Add(GenerateLink(LinkOperation.OtherPages, string.Empty, searchTerm, i, list.PageSize, sortColumn, sortOrder));
                }
            }
        }

        public void GenerateHateoasLinks<T, TKey>(T readDto, string? id = null) where T : class, ILinks<TKey>
        {
            readDto.Links.Add(GenerateLink(LinkOperation.Get, id));
            readDto.Links.Add(GenerateLink(LinkOperation.Create, id));
            readDto.Links.Add(GenerateLink(LinkOperation.Update, id));
            readDto.Links.Add(GenerateLink(LinkOperation.Delete, id));
        }

    }
}
