using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IWorkspaceRepository: IBaseBasicRepository<Guid, Workspace>
    {
        #region Domain-Specific interface


        #endregion
    }
}
