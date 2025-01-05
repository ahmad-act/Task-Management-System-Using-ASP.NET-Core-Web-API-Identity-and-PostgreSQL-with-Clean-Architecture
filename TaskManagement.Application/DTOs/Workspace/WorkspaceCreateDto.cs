using TaskManagement.Application.DTOs.BaseDTOs;

namespace TaskManagement.Application.DTOs.Workspace
{
    public class WorkspaceCreateDto : BaseCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
