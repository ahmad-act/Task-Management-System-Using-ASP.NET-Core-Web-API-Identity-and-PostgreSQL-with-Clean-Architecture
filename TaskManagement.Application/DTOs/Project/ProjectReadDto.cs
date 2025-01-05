using TaskManagement.Application.DTOs.BaseDTOs;
using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Application.DTOs.Project
{
    public class ProjectReadDto : BaseReadDto<Guid>
    {
        public Guid WorkspaceId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset ActualEndDate { get; set; }
        public string Status { get; set; }
    }
}
