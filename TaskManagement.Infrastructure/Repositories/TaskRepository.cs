using TaskManagement.Domain.Repositories;
using TaskManagement.Infrastructure.DataContext;
using TaskManagement.Infrastructure.Repositories.Base;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : BaseCommonRepository<Domain.Entities.Task>, ITaskRepository
    {
        public TaskRepository(AppDbContext dbContext)
            : base(dbContext)
        {

        }

        #region Domain-Specific Methods


        #endregion
    }
}
