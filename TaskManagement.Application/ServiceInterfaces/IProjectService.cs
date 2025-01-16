using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.ServiceInterfaces
{
    public interface IProjectService : IBaseBasicEntityService<Guid, Project, ProjectReadDto, ProjectCreateDto, ProjectUpdateDto>
    {
        Task<OptionResult<bool>> Exists(string id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<Project?>> GetByUniqueName(string name);        
    }
}
