using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Domain.Errors.Base;

namespace TaskManagement.Domain.Errors
{
    public class AppUserError : BaseError<AppUser>
    {
        public static readonly Error MissingUserName = new(400, "MISSING_USER_NAME", $"{typeof(AppUser).Name} username is missing.");
    }
}
