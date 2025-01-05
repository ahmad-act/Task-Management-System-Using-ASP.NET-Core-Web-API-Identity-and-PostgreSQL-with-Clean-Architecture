using TaskManagement.Application.DTOs.Workspace;
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.ServiceInterfaces
{
    public interface IWorkspaceService : IBaseBasicEntityService<Guid, Workspace, WorkspaceReadDto, WorkspaceCreateDto, WorkspaceUpdateDto>
    {
        Task<OptionResult<bool>> Exists(string id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<Workspace?>> GetByUniqueName(string name);        
    }
}
