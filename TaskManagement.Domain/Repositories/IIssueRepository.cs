using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IIssueRepository: IBaseEntityWithRelatedOneRepository<Guid, Issue, Project>
    {
        #region Domain-Specific interface


        #endregion
    }
}
