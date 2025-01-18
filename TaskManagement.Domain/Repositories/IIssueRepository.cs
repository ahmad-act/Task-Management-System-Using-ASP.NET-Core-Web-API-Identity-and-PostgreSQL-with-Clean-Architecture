using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IIssueRepository: IBaseBasicRepository<Guid, Issue>
    {
        #region Domain-Specific interface


        #endregion
    }
}
