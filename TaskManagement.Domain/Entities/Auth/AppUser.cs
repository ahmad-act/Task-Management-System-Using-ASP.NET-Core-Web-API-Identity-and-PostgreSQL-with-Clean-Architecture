using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Entities.Base.Common;

namespace TaskManagement.Domain.Entities.Auth
{
    public class AppUser : IdentityUser<Guid>, IBaseCommonEntity<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserType { get; set; } // Not Roles, Example- Event Attendee, Event Organizar, Event Speaker, Event Host etc.
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public virtual int? UserAccessLevel { get; set; } = 10; // Default role is least accessible user of this system.
        public virtual int? UserDataAccessLevel { get; set; } = 10; // All users of this system able to access the data.
    }
}
