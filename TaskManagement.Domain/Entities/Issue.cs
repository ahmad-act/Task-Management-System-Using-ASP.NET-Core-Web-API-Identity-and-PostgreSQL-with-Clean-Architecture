using TaskManagement.Domain.Entities.Base.Basic;

namespace TaskManagement.Domain.Entities
{
    public class Issue : BaseBasicEntity<Guid>
    {
        public Guid ProjectId { get; set; }

        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset ActualEndDate { get; set; }
        public string Status { get; set; }


        // Navigation Property
        public virtual Project? Project { get; set; }
    }
}
