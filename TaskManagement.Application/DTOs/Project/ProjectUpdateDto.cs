﻿using TaskManagement.Application.DTOs.BaseDTOs;

namespace TaskManagement.Application.DTOs.Project
{
    public class ProjectUpdateDto : BaseUpdateDto<Guid>
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
