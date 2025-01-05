using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Common.Pagination
{
    /// <summary>
    /// Represents filtering, sorting, and pagination parameters for retrieving a paginated list of entities.
    /// </summary>
    public class ListFilter
    {
        /// <summary>
        /// Gets or sets an optional search term to filter results.
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the page number for pagination.
        /// Must be greater than 0.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int? Page { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page for pagination.
        /// Must be between 1 and 100.
        /// </summary>
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int? PageSize { get; set; }

        /// <summary>
        /// Gets or sets the column name by which to sort the results.
        /// The column name must not exceed 50 characters.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Sort column name is too long.")]
        public string? SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the sort order for the results.
        /// Must be either "asc" for ascending order or "desc" for descending order.
        /// </summary>
        [RegularExpression("asc|desc", ErrorMessage = "Sort order must be 'asc' or 'desc'.")]
        public string? SortOrder { get; set; }
    }
}
