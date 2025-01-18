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


namespace TaskManagement.Infrastructure.Repositories.Base
{
    /// <summary>
    /// Provides basic repository functionality for CRUD operations with pagination and filtering.
    /// </summary>
    /// <typeparam name="T">The type of entity managed by the repository.</typeparam>
    public abstract class BaseBasicWithRelatedOneRepository<TKey, T, TRelated> : BaseBasicRepository<TKey, T>, IBaseBasicWithRelatedOneRepository<TKey, T, TRelated>
        where T : class, IBaseBasicEntity<TKey>
    {
        protected readonly AppDbContext _dbContext;

        // <summary>
        /// Initializes a new instance of the <see cref="BaseBasicRepository{T}"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context for interacting with the database.</param>
        public BaseBasicWithRelatedOneRepository(AppDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<T?> GetWithRelatedOneAsync(TKey id, Expression<Func<T, TRelated>> foreignKeyNavigation, Expression<Func<T, TKey>>? primaryKeySelector = null)
        //{
        //    var query = _dbContext.Set<T>().AsQueryable();

        //    // Include the related entity based on the foreign key navigation property
        //    query = query.Include(foreignKeyNavigation);

        //    // Determine the primary key property name
        //    var primaryKeyProperty = primaryKeySelector != null
        //        ? GetPropertyName(primaryKeySelector)
        //        : "Id"; // Default to "Id" if no selector is provided

        //    // Use the extracted property name to query the entity
        //    return await query.FirstOrDefaultAsync(entity => EqualityComparer<TKey>.Default.Equals(EF.Property<TKey>(entity, primaryKeyProperty), id));

        //}

        public async Task<T?> GetWithRelatedOneAsync(TKey id, Expression<Func<T, TRelated>> foreignKeyNavigation, Expression<Func<T, TKey>>? primaryKeySelector = null)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            // Include the related entity based on the foreign key navigation property
            query = query.Include(foreignKeyNavigation);

            // Determine the primary key property name
            string primaryKeyProperty = primaryKeySelector != null
                ? GetPropertyName(primaryKeySelector) // Extract property name
                : "Id"; // Default to "Id" if no selector is provided

            // Query the entity with the given id
            return await query.FirstOrDefaultAsync(entity =>
                EF.Property<TKey>(entity, primaryKeyProperty).Equals(id)); // Use Equals for comparison
        }



        // Helper method to extract the property name from an Expression
        private string GetPropertyName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (expression.Body is UnaryExpression unaryExpression &&
                     unaryExpression.Operand is MemberExpression memberOperand)
            {
                return memberOperand.Member.Name;
            }

            throw new ArgumentException("Invalid expression format. Could not extract property name.");
        }
    }
}