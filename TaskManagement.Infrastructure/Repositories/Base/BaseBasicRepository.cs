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
    public abstract class BaseBasicRepository<TKey, T> : BaseCommonRepository<TKey, T>, IBaseBasicRepository<TKey, T>
        where T : class, IBaseBasicEntity<TKey>
    {
        protected readonly AppDbContext _dbContext;

        // <summary>
        /// Initializes a new instance of the <see cref="BaseBasicRepository{T}"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context for interacting with the database.</param>
        public BaseBasicRepository(AppDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
