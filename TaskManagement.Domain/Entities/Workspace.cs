using TaskManagement.Domain.Entities.Base.Basic;

namespace TaskManagement.Domain.Entities
{
    public class Workspace : BaseBasicEntity<Guid>
    {
        public string? Description { get; set; }

        public virtual ICollection<Project>? Projects { get; set; }
    }
}
