
namespace TaskManagement.Application.DTOs.BaseDTOs
{
    public abstract class BaseCreateDto : IBaseCreateDto
    {
        public int? UserDataAccessLevel { get; set; }
    }
}
