using System.Linq.Expressions;
using TaskManagement.Domain.Common.Pagination;

namespace TaskManagement.Domain.Repositories.IBase
{
    public interface IBaseAuthRepository<T>
    {
        /// <summary>
        /// Retrieves a paginated list of users based on the provided search criteria.
        /// </summary>
        /// <param name="searchTerm">An optional search term to filter users by name or other properties.</param>
        /// <param name="page">The page number to retrieve (zero-based index).</param>
        /// <param name="pageSize">The number of users to return per page.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortOrder">The order of sorting; can be "asc" or "desc".</param>
        /// <returns>A task representing the asynchronous operation, containing a paginated list of users.</returns>
        Task<IPaginatedList<T>> ListAsync(string? searchTerm = null, int? page = null, int? pageSize = null, string? sortColumn = null, string? sortOrder = null, Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Retrieves a entity by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task representing the asynchronous operation, containing the entity if found; otherwise, <c>null</c>.</returns>
        Task<T?> GetAsync(string id);

        /// <summary>
        /// Creates a new entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>A task representing the asynchronous operation, containing the number of records created.</returns>
        Task<int> CreateAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the data store.
        /// </summary>
        /// <param name="entity">The entity with updated information.</param>
        /// <returns>A task representing the asynchronous operation, containing the number of records updated.</returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Deletes a entity from the data store.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A task representing the asynchronous operation, containing the number of records deleted.</returns>
        Task<int> DeleteAsync(T entity);

        /// <summary>
        /// Checks if a user exists by their username.
        /// </summary>
        /// <param name="username">The username to check for existence.</param>
        /// <returns>A task representing the asynchronous operation, containing <c>true</c> if the user exists; otherwise, <c>false</c>.</returns>
        Task<bool> UserExistsAsync(string username);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the user if found; otherwise, <c>null</c>.</returns>
        Task<T?> GetUserByUsernameAsync(string username);
    }
}
