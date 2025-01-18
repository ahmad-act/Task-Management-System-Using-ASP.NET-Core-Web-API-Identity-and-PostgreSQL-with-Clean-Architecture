using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Entities.Base.Common;

namespace TaskManagement.Domain.Entities.Base.Basic
{
    /// <summary>
    /// Defines the contract for a base basic (where have ID and Name property) entity with a primary key.
    /// </summary>
    public interface IBaseBasicEntity<TKey> : IBaseCommonEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the unique name for the entity.
        /// </summary>
        [Required]
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the description for the entity.
        /// </summary>
        public string? Description { get; set; }
    }
}