using TaskManagement.Application.DTOs.BaseDTOs;

namespace TaskManagement.Application.DTOs.EntityPrototype
{
    public class EntityPrototypeCreateDto : BaseCreateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
