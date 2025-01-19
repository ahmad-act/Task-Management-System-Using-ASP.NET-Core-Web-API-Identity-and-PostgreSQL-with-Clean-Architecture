using TaskManagement.Application.DTOs.BaseDTOs;
using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Application.DTOs.Task
{
    using TaskManagement.Domain.Entities;

    public class TaskReadDto : BaseReadDto<Guid>
    {
        public Guid ProjectId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset ActualEndDate { get; set; }
        public string Status { get; set; }
        public Project? Project { get; set; }
    }
}
