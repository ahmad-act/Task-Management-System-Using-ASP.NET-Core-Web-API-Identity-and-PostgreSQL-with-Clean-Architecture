
namespace TaskManagement.Domain.ValueObjects
{
    /// <summary>
    /// Represents a unique identifier for an entity.
    /// Supports any type that has a default constructor, with <see cref="Guid"/> as the default.
    /// </summary>
    public class EntityID<T> where T : class, new()
    {
        private T? _entityId;

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// If the value is not provided, it generates a new identifier based on the type.
        /// </summary>
        public T EntityId
        {
            get => _entityId ??= GenerateNewId();
            set => _entityId = value ?? GenerateNewId();
        }

        /// <summary>
        /// Returns the entity's ID as a string.
        /// </summary>
        public override string ToString() => EntityId?.ToString() ?? string.Empty;

        /// <summary>
        /// Implicitly converts a string representation of an identifier to an EntityID.
        /// Only works if T is Guid.
        /// </summary>
        public static implicit operator EntityID<T>(string id)
        {
            return new EntityID<T>
            {
                EntityId = (T)(object)Guid.Parse(id) // Cast to T if T is Guid
            };
        }

        /// <summary>
        /// Implicitly converts an EntityID to its string representation.
        /// </summary>
        public static implicit operator string(EntityID<T> entityID)
        {
            return entityID.EntityId?.ToString() ?? string.Empty;
        }

        // Helper method to generate a new ID based on the type
        private T GenerateNewId()
        {
            return typeof(T) switch
            {
                Type t when t == typeof(Guid) => (T)(object)Guid.NewGuid(),   // Generate and cast Guid as T
                Type t when t == typeof(Ulid) => (T)(object)Ulid.NewUlid(),   // Generate and cast Guid as T
                _ => new T() // If T is neither Guid nor Guid, create a new instance of T
            };
        }
    }
}
