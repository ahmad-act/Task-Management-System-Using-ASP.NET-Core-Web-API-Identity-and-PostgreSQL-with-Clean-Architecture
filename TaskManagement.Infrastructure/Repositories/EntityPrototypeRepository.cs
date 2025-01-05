using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories;
using TaskManagement.Infrastructure.DataContext;
using TaskManagement.Infrastructure.Repositories.Base;

namespace TaskManagement.Infrastructure.Repositories
{
    public class EntityPrototypeRepository : BaseCommonRepository<EntityPrototype>, IEntityPrototypeRepository
    {
        public EntityPrototypeRepository(AppDbContext dbContext)
            : base(dbContext)
        {

        }

        #region Domain-Specific Methods


        #endregion
    }
}
