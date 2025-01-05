namespace TaskManagement.Application.DTOs.AuthDTOs.AppRole
{
    using TaskManagement.Application.DTOs.BaseDTOs;
    using TaskManagement.Domain.Entities.Auth;

    public class AppRoleReadDto : BaseReadDto<Guid>
    {
        public AppRole AppRole { get; set; }
        public List<AppUser> AppUsers { get; set; }
    }
}
