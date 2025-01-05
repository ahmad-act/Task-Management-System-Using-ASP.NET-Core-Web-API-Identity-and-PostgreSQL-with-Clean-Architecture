using TaskManagement.Application.DTOs.Workspace;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Entities;
using TaskManagement.Presenter.Controllers.Base.v1;

namespace TaskManagement.Presenter.Controllers.v1
{
    public class WorkspaceController : BaseBasicEntityController<Guid, Workspace, WorkspaceReadDto, WorkspaceCreateDto, WorkspaceUpdateDto>
    {
        private readonly IWorkspaceService _service;

        public WorkspaceController(IWorkspaceService service, JwtSettings jwtSettings)
            : base(service, jwtSettings)
        {
            _service = service;
        }

        #region Domain-Specific endpoints


        #endregion
    }
}
