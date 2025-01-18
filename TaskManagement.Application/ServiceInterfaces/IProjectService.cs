using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.ServiceInterfaces
{
    public interface IProjectService : IBaseBasicWithRelatedOneService<Guid, Project, Workspace, ProjectReadDto, ProjectCreateDto, ProjectUpdateDto>
    {
        Task<OptionResult<bool>> Exists(string id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<Project?>> GetByUniqueName(string name);
        Task<OptionResult<Project?>> GetAsync(string id);
    }
}
