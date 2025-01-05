using TaskManagement.Application.DTOs.Issue;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Entities;
using TaskManagement.Presenter.Controllers.Base.v1;

namespace TaskManagement.Presenter.Controllers.v1
{
    public class IssueController : BaseBasicEntityController<Guid, Issue, IssueReadDto, IssueCreateDto, IssueUpdateDto>
    {
        private readonly IIssueService _service;

        public IssueController(IIssueService service, JwtSettings jwtSettings)
            : base(service, jwtSettings)
        {
            _service = service;
        }

        #region Domain-Specific endpoints


        #endregion
    }
}
