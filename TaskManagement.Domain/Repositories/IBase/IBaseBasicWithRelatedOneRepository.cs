using System.Linq.Expressions;

namespace TaskManagement.Domain.Repositories.IBase
{
    public interface IBaseBasicWithRelatedOneRepository<TKey, T, TRelated> : IBaseBasicRepository<TKey, T>
    {
        Task<T?> GetWithRelatedOneAsync(TKey id, Expression<Func<T, TRelated>> foreignKeyNavigation, Expression<Func<T, TKey>>? primaryKeySelector = null);
    }
}
