
using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Application.DTOs.BaseDTOs
{
    public abstract class BaseUpdateDto<TKey> : IBaseUpdateDto
    {
        public int? UserDataAccessLevel { get; set; }
    }
}
