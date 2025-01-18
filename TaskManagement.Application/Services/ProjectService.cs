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
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Domain.Common.ReturnType;

namespace TaskManagement.Application.Services
{
    public class ProjectService : BaseBasicWithRelatedOneService<Guid, Project, Workspace, ProjectReadDto, ProjectCreateDto, ProjectUpdateDto>, IProjectService
    {
        private readonly IProjectRepository _repository;

        public ProjectService(IActivityLog activityLog, IProjectRepository repository, IMapper mapper, JwtSettings jwtSettings, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IValidator<ProjectCreateDto> createValidator, IValidator<ProjectUpdateDto> updateValidator)
            : base(activityLog, repository, entityLinkGenerator, appUserService, mapper, createValidator, updateValidator)
        {
            _repository = repository;
        }

        #region Common Methods

        public async Task<OptionResult<bool>> Exists(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new[] { ProjectError.MissingId };
            }

            var existingEntity = await _repository.GetAsync(Guid.Parse(id));

            if (existingEntity == null)
            {
                return new[] { ProjectError.NotFound };
            }

            return true;
        }

        public async Task<OptionResult<bool>> ExistsByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { ProjectError.MissingTitle };
            }

            Expression<Func<Project, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.Exists(predicate);
        }

        public async Task<OptionResult<Project?>> GetByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { ProjectError.MissingTitle };
            }

            Expression<Func<Project, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.GetByCondition(predicate);
        }

        #endregion

        public async Task<OptionResult<Project?>> GetAsync(string id)
        {

            var orderWithCustomer = await _repository.GetWithRelatedOneAsync(
                id: Guid.Parse(id),
                foreignKeyNavigation: fk => fk.Workspace,
                primaryKeySelector: fk => fk.Id
            );


            return orderWithCustomer;
        }




        #region Domain-Specific Methods


        #endregion
    }
}
