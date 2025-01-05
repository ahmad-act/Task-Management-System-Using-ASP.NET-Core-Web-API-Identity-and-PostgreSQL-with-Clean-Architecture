using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common;
using TaskManagement.Application.DTOs.Task;

namespace TaskManagement.Application.ServiceInterfaces
{
    public interface ITaskService : IBaseBasicEntityService<Guid, Domain.Entities.Task, TaskReadDto, TaskCreateDto, TaskUpdateDto>
    {
        Task<OptionResult<bool>> Exists(string id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<Domain.Entities.Task?>> GetByUniqueName(string name);        
    }
}
