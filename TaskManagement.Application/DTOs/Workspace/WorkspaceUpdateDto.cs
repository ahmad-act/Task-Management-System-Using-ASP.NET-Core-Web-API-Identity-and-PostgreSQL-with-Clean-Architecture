using TaskManagement.Application.DTOs.BaseDTOs;

namespace TaskManagement.Application.DTOs.Workspace
{
    public class WorkspaceUpdateDto : BaseUpdateDto<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
