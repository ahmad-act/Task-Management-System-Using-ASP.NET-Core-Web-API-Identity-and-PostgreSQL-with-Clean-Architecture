using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IProjectRepository: IBaseEntityWithRelatedOneRepository<Guid, Project, Workspace>
    {
        #region Domain-Specific interface


        #endregion
    }
}
