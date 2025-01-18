using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories.IBase;

namespace TaskManagement.Domain.Repositories
{
    public interface IEntityPrototypeRepository: IBaseBasicRepository<Guid, EntityPrototype>
    {
        #region Domain-Specific interface


        #endregion
    }
}
