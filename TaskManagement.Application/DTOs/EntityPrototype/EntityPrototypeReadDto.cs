using TaskManagement.Application.DTOs.BaseDTOs;
using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Application.DTOs.EntityPrototype
{
    public class EntityPrototypeReadDto : BaseReadDto<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
