using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;
using Task = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Domain.Repositories
{
    public interface ITaskRepository: IBaseEntityWithRelatedOneRepository<Guid, Task, Project>
    {
        #region Domain-Specific interface


        #endregion
    }
}
