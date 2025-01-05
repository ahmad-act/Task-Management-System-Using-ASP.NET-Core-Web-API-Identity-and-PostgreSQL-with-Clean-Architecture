using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Entities.Base.Common;

namespace TaskManagement.Domain.Entities.Auth
{
    public class AppRole : IdentityRole<Guid>, IBaseCommonEntity<Guid>
    {
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public virtual int? UserDataAccessLevel { get; set; } = 10; // All users of this system able to access the data.
    }
}
