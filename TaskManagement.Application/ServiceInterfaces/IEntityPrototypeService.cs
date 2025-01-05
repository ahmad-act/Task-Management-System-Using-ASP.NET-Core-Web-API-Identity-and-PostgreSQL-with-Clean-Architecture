using TaskManagement.Application.DTOs.EntityPrototype;
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.ServiceInterfaces
{
    public interface IEntityPrototypeService : IBaseBasicEntityService<Guid, EntityPrototype, EntityPrototypeReadDto, EntityPrototypeCreateDto, EntityPrototypeUpdateDto>
    {
        Task<OptionResult<bool>> Exists(string id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<EntityPrototype?>> GetByUniqueName(string name);        
    }
}
