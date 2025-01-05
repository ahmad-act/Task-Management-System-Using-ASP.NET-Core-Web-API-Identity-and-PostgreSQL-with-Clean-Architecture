using System.Security.Claims;
using TaskManagement.Application.DTOs.BaseDTOs;

namespace TaskManagement.Application.DTOs.AuthDTOs.AppUser
{
    public class AppUserCreateDto : BaseCreateDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> PolicyNames { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
