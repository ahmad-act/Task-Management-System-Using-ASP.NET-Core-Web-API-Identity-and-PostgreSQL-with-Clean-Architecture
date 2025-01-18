// System
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// Domain/Core layer
using TaskManagement.Domain.Repositories.IBase;
using TaskManagement.Domain.Common.Pagination;

// Application Layer
using TaskManagement.Application.Utilities.Pagination;

// Infrastructure Layer
using TaskManagement.Infrastructure.DataContext;
using TaskManagement.Domain.Entities.Base.Common;
using MailKit.Search;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Repositories.Base
{
    /// <summary>
    /// Provides basic repository functionality for CRUD operations with pagination and filtering.
    /// </summary>
    /// <typeparam name="T">The type of entity managed by the repository.</typeparam>
    public abstract class BaseCommonRepository<TKey, T> : IBaseCommonRepository<TKey, T>
        where T : class, IBaseCommonEntity<TKey>
    {
        protected readonly AppDbContext _dbContext;

        // <summary>
        /// Initializes a new instance of the <see cref="BaseCommonRepository{T}"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context for interacting with the database.</param>
        public BaseCommonRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves a paginated list of entities with optional filtering, sorting, and pagination.
        /// </summary>
        /// <param name="listFilter">
        /// An instance of <see cref="ListFilter"/> containing optional parameters:
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
        public async Task<IPaginatedList<T>> ListAsync(ListFilter listFilter, Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbContext.Set<T>()
                .AsNoTracking()
                .AsSplitQuery(); // Use when more that two join in the query 

            // Apply filtering based on the provided filter expression
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(listFilter.SortColumn))
            {
                var isDescending = listFilter.SortOrder?.ToLower() == "desc";
                query = isDescending
                    ? query.OrderByDescending(_dbContext.GetSortExpression<T>(listFilter.SortColumn))
                    : query.OrderBy(_dbContext.GetSortExpression<T>(listFilter.SortColumn));
            }

            int totalRecords = await query.CountAsync();

            uint pageNum = listFilter.Page ?? 1;
            uint recordsPerPage = listFilter.PageSize ?? (uint)totalRecords;

            // Return all records if page or pageSize is not specified
            if (pageNum == 0 || recordsPerPage == 0)
            {
                recordsPerPage = (uint)totalRecords;
                pageNum = (uint)(recordsPerPage == 0 ? 0 : 1);
            }

            return await PaginatedList<T>.CreateAsync(query, pageNum, recordsPerPage);
        }

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The entity of type <typeparamref name="T"/>, or null if not found.</returns>
        public async Task<T?> GetAsync(TKey id)
        {







            return await _dbContext.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// </summary>
        /// <param name="ids">The IDs of the entity to retrieve.</param>
        /// <returns>The entity of type <typeparamref name="T"/>, or null if not found.</returns>
        public async Task<List<T>> GetAsync(IEnumerable<TKey> ids)
        {
            return await _dbContext.Set<T>()
                .Where(entity => entity.Id != null && ids.Contains(entity.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new entity and saves it to the database.
        /// </summary>
        /// <param name="entity">The entity to create and save.</param>
        /// <returns>The number of affected rows (1 if successful, 0 if failed).</returns>
        public async Task<int> CreateAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The number of affected rows (1 if successful, 0 if failed).</returns>
        public async Task<int> UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The number of affected rows (1 if successful, 0 if failed).</returns>
        public async Task<int> DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Checks whether an entity matching the specified condition exists.
        /// </summary>
        /// <param name="predicate">The condition to check.</param>
        /// <returns>True if any entity matches the condition, otherwise false.</returns>
        public async Task<bool> Exists(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }

        /// <summary>
        /// Retrieves a single entity that matches the specified condition.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        public async Task<T?> GetByCondition(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }












    }
}
