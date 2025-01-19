using System.Linq.Expressions;
using TaskManagement.Domain.Common.Pagination;
using TaskManagement.Domain.Common.ReturnType;

namespace TaskManagement.Application.ServiceInterfaces.IBase
{
    /// <summary>
    /// Defines the base service operations for managing entities, including CRUD operations and pagination.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TReadDto">The DTO used to read the entity data.</typeparam>
    /// <typeparam name="TCreateDto">The DTO used for creating the entity.</typeparam>
    /// <typeparam name="TUpdateDto">The DTO used for updating the entity.</typeparam>
    public interface IBaseEntityWithRelatedOneService<TKey, T, TRelated, TReadDto, TCreateDto, TUpdateDto>
    {
        #region CRUD Operations

        /// <summary>
        /// Retrieves a paginated list of entities with optional filtering, sorting, and pagination.
        /// </summary>
        /// <param name="queryParams">
        /// An instance of <see cref="QueryParams"/> containing optional parameters:
        /// <list type="bullet">
        /// <item>
        /// <description><c>SearchTerm</c>: Optional search term for filtering results.</description>
        /// </item>
        /// <item>
        /// <description><c>Page</c>: Optional page number for pagination. Must be greater than 0.</description>
        /// </item>
        /// <item>
        /// <description><c>PageSize</c>: Optional page size for pagination. Must be between 1 and 100.</description>
        /// </item>
        /// <item>
        /// <description><c>SortColumn</c>: Optional column to sort by. Maximum length is 50 characters.</description>
        /// </item>
        /// <item>
        /// <description><c>SortOrder</c>: Optional sort order. Valid values are "asc" or "desc".</description>
        /// </item>
        /// </list>
        /// </param>
        /// <typeparam name="TReadDto">The type of the data transfer object used to represent the entities.</typeparam>
        /// <typeparam name="TKey">The type of the unique identifier for entities.</typeparam>
        /// <returns>
        /// An <see cref="OptionResult{T}"/> containing:
        /// <list type="bullet">
        /// <item>
        /// <description>A paginated list of <typeparamref name="TReadDto"/> representing the entities if successful.</description>
        /// </item>
        /// <item>
        /// <description>An error code and message if the operation fails.</description>
        /// </item>
        /// </list>
        /// </returns>
        Task<OptionResult<IPaginatedList<TReadDto>>> ListAsync(QueryParams queryParams, Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The <typeparamref name="TReadDto"/> representing the entity, or an error if the entity is not found.</returns>
        Task<OptionResult<TReadDto>> GetAsync(TKey id);

        /// <summary>
        /// Creates a new entity based on the provided create DTO.
        /// </summary>
        /// <param name="create">The DTO containing data for creating the new entity.</param>
        /// <returns>The ID of the newly created entity.</returns>
        Task<OptionResult<TKey>> CreateAsync(TCreateDto create);

        /// <summary>
        /// Updates an existing entity by its ID using the provided update DTO.
        /// </summary>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="update">The DTO containing the updated data for the entity.</param>
        /// <returns>The updated DTO representing the entity.</returns>
        Task<OptionResult<TUpdateDto>> UpdateAsync(TKey id, TUpdateDto update);

        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>True if the entity was deleted successfully, otherwise false.</returns>
        Task<OptionResult<bool>> DeleteAsync(TKey id);

        #endregion

        #region Common Methods

        Task<OptionResult<bool>> ExistsByUniqueName(string name);

        Task<OptionResult<T?>> GetByUniqueName(string name);

        #endregion

        #region Get entitity with realated one using FK

        Task<OptionResult<IPaginatedList<TReadDto>>> ListWithRelatedOneAsync(QueryParams queryParams, Expression<Func<T, bool>>? filter = null);
        Task<TReadDto?> GetWithRelatedOneAsync(TKey id);

        #endregion
    }
}
