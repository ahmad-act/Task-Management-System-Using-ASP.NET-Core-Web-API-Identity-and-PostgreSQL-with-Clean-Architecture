using System.Security.Claims;

namespace TaskManagement.Application.DTOs.AuthDTOs.AppUser
{
    using System.Collections.Generic;
    using TaskManagement.Domain.Common.HATEOAS;
    using TaskManagement.Domain.Entities.Auth;

    public class AppUserReadDto : ILinks<Guid>
    {
        public Guid Id { get; set; }
        public AppUser AppUser { get; set; }
        public List<AppRole> AppRoles { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Policies { get; set; }
        public IList<Link> Links { get; set; }
    }
}
