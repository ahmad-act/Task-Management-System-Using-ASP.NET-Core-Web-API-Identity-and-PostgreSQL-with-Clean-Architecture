using TaskManagement.Application.DTOs.BaseDTOs;
using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Application.DTOs.Workspace
{
    public class WorkspaceReadDto : BaseReadDto<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
