using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IUserRepository: IBaseAuthRepository<AppUser>
    {

    }
}
