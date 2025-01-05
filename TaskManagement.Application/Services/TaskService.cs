// System
using System.Linq.Expressions;

// Third-party libraries
using AutoMapper;
using FluentValidation;

// Domain/Core layer
using TaskManagement.Domain.Repositories;
using TaskManagement.Domain.Errors;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Common.AuditLog;
using TaskManagement.Domain.Common.HATEOAS;
using TaskManagement.Domain.Common.JWT;

// Application Layer
using TaskManagement.Application.Services.Base;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Application.DTOs.Task;

namespace TaskManagement.Application.Services
{
    public class TaskService : BaseBasicEntityService<Guid, Domain.Entities.Task, TaskReadDto, TaskCreateDto, TaskUpdateDto>, ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(IActivityLog activityLog, ITaskRepository repository, IMapper mapper, JwtSettings jwtSettings, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IValidator<TaskCreateDto> createValidator, IValidator<TaskUpdateDto> updateValidator)
            : base(activityLog, repository, entityLinkGenerator, appUserService, mapper, createValidator, updateValidator)
        {
            _repository = repository;
        }

        #region Common Methods

        public async Task<OptionResult<bool>> Exists(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return TaskError.MissingId;
            }

            var existingEntity = await _repository.GetAsync(id);

            if (existingEntity == null)
            {
                return TaskError.NotFound;
            }

            return true;
        }

        public async Task<OptionResult<bool>> ExistsByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return TaskError.MissingTitle;
            }

            Expression<Func<Domain.Entities.Task, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.Exists(predicate);
        }

        public async Task<OptionResult<Domain.Entities.Task?>> GetByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return TaskError.MissingTitle;
            }

            Expression<Func<Domain.Entities.Task, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.GetByCondition(predicate);
        }

        #endregion

        #region Domain-Specific Methods


        #endregion
    }
}
