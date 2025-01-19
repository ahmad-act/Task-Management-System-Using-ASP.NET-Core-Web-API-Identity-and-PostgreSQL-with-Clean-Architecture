// System
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// Domain/Core layer
using TaskManagement.Domain.Repositories.IBase;
using TaskManagement.Domain.Entities.Base.Basic;

// Application Layer
using TaskManagement.Application.Utilities.Pagination;

// Infrastructure Layer
using TaskManagement.Infrastructure.DataContext;
using TaskManagement.Application.Utilities.ExtensionMethods;
using TaskManagement.Domain.Common.Pagination;


namespace TaskManagement.Infrastructure.Repositories.Base
{
    /// <summary>
    /// Provides basic repository functionality for CRUD operations with pagination and filtering.
    /// </summary>
    /// <typeparam name="T">The type of entity managed by the repository.</typeparam>
    public abstract class BaseEntityWithRelatedOneRepository<TKey, T, TRelated> : BaseBasicRepository<TKey, T>, IBaseEntityWithRelatedOneRepository<TKey, T, TRelated>
        where T : class, IBaseBasicEntity<TKey>
    {
        protected readonly AppDbContext _dbContext;

        // <summary>
        /// Initializes a new instance of the <see cref="BaseBasicRepository{T}"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context for interacting with the database.</param>
        public BaseEntityWithRelatedOneRepository(AppDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IPaginatedList<T>> ListWithRelatedOneAsync(Expression<Func<T, TRelated>> foreignKeyNavigation, ListFilter listFilter, Expression<Func<T, bool>>? filter = null)
        {
            var query = _dbContext.Set<T>()
                .AsQueryable()
                .AsSplitQuery();

            // Include the related entity based on the foreign key navigation property
            query = query.Include(foreignKeyNavigation);


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


            // Return all entities along with their related data
           // return await query.ToListAsync();
        }

        public async Task<T?> GetWithRelatedOneAsync(TKey id, Expression<Func<T, TRelated>> foreignKeyNavigation, Expression<Func<T, TKey>>? primaryKeySelector = null)
        {
            var query = _dbContext.Set<T>()
                .AsQueryable();

            // Include the related entity based on the foreign key navigation property
            query = query.Include(foreignKeyNavigation);

            // Determine the primary key property name
            string primaryKeyProperty = primaryKeySelector != null
                ? primaryKeySelector.GetPropertyName() // Extract property name
                : "Id"; // Default to "Id" if no selector is provided

            // Query the entity with the given id
            return await query.FirstOrDefaultAsync(entity =>
                EF.Property<TKey>(entity, primaryKeyProperty) != null &&
                EF.Property<TKey>(entity, primaryKeyProperty)!.Equals(id));
        }
    }
}