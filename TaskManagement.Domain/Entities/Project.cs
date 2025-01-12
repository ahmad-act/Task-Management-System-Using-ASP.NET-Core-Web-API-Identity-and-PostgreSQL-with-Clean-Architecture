using TaskManagement.Domain.Entities.Base.Basic;

namespace TaskManagement.Domain.Entities
{
    public class Project : BaseBasicEntity<Guid>
    {
        public Guid WorkspaceId { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset ActualEndDate { get; set; }
        public string Status { get; set; }


        // Navigation Property
        public virtual Workspace? Workspace { get; set; }
        public virtual ICollection<Task>? Task { get; set; }
        public virtual ICollection<Issue>? Issue { get; set; }
    }
}
