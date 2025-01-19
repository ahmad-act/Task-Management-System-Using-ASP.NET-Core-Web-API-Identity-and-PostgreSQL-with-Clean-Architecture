using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq.Expressions;
using TaskManagement.Domain.Common.Pagination;

namespace TaskManagement.Domain.Repositories.IBase
{
    public interface IBaseCommonRepository<TKey, T>
    {
        #region CURD Operation 

        /// <summary>
        /// Retrieves a paginated list of entities based on the provided search criteria.
        /// </summary>
        /// <param name="searchTerm">An optional search term to filter entities by name or other properties.</param>
        /// <param name="page">The page number to retrieve (zero-based index).</param>
        /// <param name="pageSize">The number of entities to return per page.</param>
        /// <param name="sortColumn">The column to sort the results by.</param>
        /// <param name="sortOrder">The order of sorting; can be "asc" or "desc".</param>
        /// <param name="filter">An optional filter expression to filter the entities.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing a paginated list of entities.
        /// </returns>
        Task<IPaginatedList<T>> ListAsync(QueryParams queryParams, Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing the entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<T?> GetAsync(TKey id);

        Task<List<T>> GetAsync(IEnumerable<TKey> ids);

        /// <summary>
        /// Creates a new entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing the number of records created.
        /// </returns>
        Task<int> CreateAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the data store.
        /// </summary>
        /// <param name="entity">The entity with updated information.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing the number of records updated.
        /// </returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity from the data store.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing the number of records deleted.
        /// </returns>        
        Task<int> DeleteAsync(T entity);

        #endregion

        #region Common Methods

        /// <summary>
        /// Checks if an entity exists in the data store based on a given predicate.
        /// </summary>
        /// <param name="predicate">The condition to check for existence.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing <c>true</c> if an entity matching the predicate exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves a single entity based on a specified condition.
        /// </summary>
        /// <param name="predicate">The condition to retrieve the entity.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing the entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<T?> GetByCondition(Expression<Func<T, bool>> predicate);

        #endregion
    }
}
