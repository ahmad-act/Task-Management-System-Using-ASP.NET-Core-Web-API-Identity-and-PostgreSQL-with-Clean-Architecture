using System.Linq.Expressions;
using TaskManagement.Domain.Common.Pagination;

namespace TaskManagement.Domain.Repositories.IBase
{
    public interface IBaseEntityWithRelatedOneRepository<TKey, T, TRelated> : IBaseBasicRepository<TKey, T>
    {
        #region Get entitity with realated one using FK

        Task<IPaginatedList<T>> ListWithRelatedOneAsync(Expression<Func<T, TRelated>> foreignKeyNavigation, QueryParams queryParams, Expression<Func<T, bool>>? filter = null);
        Task<T?> GetWithRelatedOneAsync(TKey id, Expression<Func<T, TRelated>> foreignKeyNavigation, Expression<Func<T, TKey>>? primaryKeySelector = null);

        #endregion
    }
}
