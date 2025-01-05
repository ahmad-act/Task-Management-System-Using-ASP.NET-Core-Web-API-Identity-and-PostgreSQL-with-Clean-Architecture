namespace TaskManagement.Domain.Entities.Base.Common
{
    /// <summary>
    /// Represents the base class for all entities in the system.
    /// Provides common properties that are shared across entities.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for the entity.</typeparam>
    public abstract class BaseCommonEntity<TKey> : IBaseCommonEntity<TKey>
    {
        public virtual TKey CreatedBy { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual DateTimeOffset ModifiedAt { get; set; } = DateTime.UtcNow;
        public virtual int? UserDataAccessLevel { get; set; } = 10; // All users of this system able to access the data.
    }
}