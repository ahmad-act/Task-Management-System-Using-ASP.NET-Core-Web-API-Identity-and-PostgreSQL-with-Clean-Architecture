// System
using System.Linq.Expressions;

// Third-party libraries
using AutoMapper;
using FluentValidation;

// Domain/Core layer
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories;
using TaskManagement.Domain.Errors;
using TaskManagement.Domain.Common.AuditLog;
using TaskManagement.Domain.Common.HATEOAS;
using TaskManagement.Domain.Common.JWT;

// Application Layer
using TaskManagement.Application.Services.Base;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Application.DTOs.Workspace;
using TaskManagement.Domain.Common.ReturnType;

namespace TaskManagement.Application.Services
{
    public class WorkspaceService : BaseBasicEntityService<Guid, Workspace, WorkspaceReadDto, WorkspaceCreateDto, WorkspaceUpdateDto>, IWorkspaceService
    {
        private readonly IWorkspaceRepository _repository;

        public WorkspaceService(IActivityLog activityLog, IWorkspaceRepository repository, IMapper mapper, JwtSettings jwtSettings, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IValidator<WorkspaceCreateDto> createValidator, IValidator<WorkspaceUpdateDto> updateValidator)
            : base(activityLog, repository, entityLinkGenerator, appUserService, mapper, createValidator, updateValidator)
        {
            _repository = repository;
        }

        #region Common Methods

        public async Task<OptionResult<bool>> Exists(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new[] { WorkspaceError.MissingId };
            }

            var existingEntity = await _repository.GetAsync(Guid.Parse(id));

            if (existingEntity == null)
            {
                return new[] { WorkspaceError.NotFound };
            }

            return true;
        }

        public async Task<OptionResult<bool>> ExistsByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { WorkspaceError.MissingTitle };
            }

            Expression<Func<Workspace, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.Exists(predicate);
        }

        public async Task<OptionResult<Workspace?>> GetByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { WorkspaceError.MissingTitle };
            }

            Expression<Func<Workspace, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.GetByCondition(predicate);
        }

        #endregion

        #region Domain-Specific Methods


        #endregion
    }
}
