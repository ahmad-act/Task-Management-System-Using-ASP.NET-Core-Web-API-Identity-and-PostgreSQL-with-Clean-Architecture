namespace TaskManagement.Domain.Entities.Base.Basic
{
    /// <summary>
    /// Represents the base basic class (where have ID and Name property) for all entities in the system.
    /// Provides common properties that are shared across entities.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for the entity.</typeparam>
    public abstract class BaseBasicEntity<TKey> : IBaseBasicEntity<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
        public TKey CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int? UserDataAccessLevel { get; set; }
    }
}