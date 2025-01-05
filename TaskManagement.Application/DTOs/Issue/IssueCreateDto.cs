using TaskManagement.Application.DTOs.BaseDTOs;

namespace TaskManagement.Application.DTOs.Issue
{
    public class IssueCreateDto : BaseCreateDto
    {
        public Guid ProjectId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset ActualEndDate { get; set; }
        public string Status { get; set; }

    }
}
