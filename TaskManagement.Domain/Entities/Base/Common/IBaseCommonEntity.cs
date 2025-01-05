namespace TaskManagement.Domain.Entities.Base.Common
{
    /// <summary>
    /// Defines the contract for a base entity with a primary key.
    /// </summary>
    public interface IBaseCommonEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        TKey CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the timestamp of when the entity was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }
        /// <summary>
        /// Gets or sets the timestamp of when the entity was last modified.
        /// </summary>
        DateTimeOffset ModifiedAt { get; set; }
        /// <summary>
        /// Gets or sets the level of access for users to the data, helping to determine which data users can access or not.
        /// </summary>
        int? UserDataAccessLevel { get; set; }
    }
}