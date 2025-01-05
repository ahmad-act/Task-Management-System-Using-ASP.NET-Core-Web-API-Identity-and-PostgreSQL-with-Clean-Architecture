using TaskManagement.Application.DTOs.EntityPrototype;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Entities;
using TaskManagement.Presenter.Controllers.Base.v1;

namespace TaskManagement.Presenter.Controllers.v1
{
    public class EntityPrototypeController : BaseBasicEntityController<Guid, EntityPrototype, EntityPrototypeReadDto, EntityPrototypeCreateDto, EntityPrototypeUpdateDto>
    {
        private readonly IEntityPrototypeService _service;

        public EntityPrototypeController(IEntityPrototypeService service, JwtSettings jwtSettings)
            : base(service, jwtSettings)
        {
            _service = service;
        }

        #region Domain-Specific endpoints


        #endregion
    }
}
