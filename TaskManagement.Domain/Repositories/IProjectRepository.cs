using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IProjectRepository: IBaseCommonRepository<Project>
    {
        #region Domain-Specific interface


        #endregion
    }
}
