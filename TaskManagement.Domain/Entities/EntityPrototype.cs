using TaskManagement.Domain.Entities.Base.Basic;

namespace TaskManagement.Domain.Entities
{
    public class EntityPrototype : BaseBasicEntity<Guid>
    {
        public string Description { get; set; }
    }
}
