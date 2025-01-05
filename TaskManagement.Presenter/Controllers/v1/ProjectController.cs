using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Entities;
using TaskManagement.Presenter.Controllers.Base.v1;

namespace TaskManagement.Presenter.Controllers.v1
{
    public class ProjectController : BaseBasicEntityController<Guid, Project, ProjectReadDto, ProjectCreateDto, ProjectUpdateDto>
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service, JwtSettings jwtSettings)
            : base(service, jwtSettings)
        {
            _service = service;
        }

        #region Domain-Specific endpoints


        #endregion
    }
}
