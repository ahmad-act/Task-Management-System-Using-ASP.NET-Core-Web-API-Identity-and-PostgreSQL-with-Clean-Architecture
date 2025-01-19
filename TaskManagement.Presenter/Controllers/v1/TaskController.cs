using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Entities;
using TaskManagement.Presenter.Controllers.Base.v1;

namespace TaskManagement.Presenter.Controllers.v1
{
    public class TaskController : BaseEntityWithRelatedOneController<Guid, Domain.Entities.Task, Project, TaskReadDto, TaskCreateDto, TaskUpdateDto>
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service, JwtSettings jwtSettings)
            : base(service, jwtSettings)
        {
            _service = service;
        }

        #region Domain-Specific endpoints


        #endregion
    }
}
