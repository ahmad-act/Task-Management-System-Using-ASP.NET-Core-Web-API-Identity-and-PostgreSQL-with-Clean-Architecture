using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories;
using TaskManagement.Infrastructure.DataContext;
using TaskManagement.Infrastructure.Repositories.Base;

namespace TaskManagement.Infrastructure.Repositories
{
    public class IssueRepository : BaseEntityWithRelatedOneRepository<Guid, Issue, Project>, IIssueRepository
    {
        public IssueRepository(AppDbContext dbContext)
            : base(dbContext)
        {

        }

        #region Domain-Specific Methods


        #endregion
    }
}
