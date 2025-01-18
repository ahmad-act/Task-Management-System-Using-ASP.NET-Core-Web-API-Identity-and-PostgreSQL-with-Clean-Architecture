using System.Linq.Expressions;

// Third-party libraries
using AutoMapper;
using FluentValidation;

// Domain/Core layer
using TaskManagement.Domain.Entities.Base.Basic;
using TaskManagement.Domain.Repositories.IBase;
using TaskManagement.Domain.Common.Pagination;
using TaskManagement.Domain.Common.AuditLog;
using TaskManagement.Domain.Common.HATEOAS;

// Application Layer
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Domain.Errors.Base;
using TaskManagement.Application.Utilities.ExtensionMethods;
using TaskManagement.Domain.Common.ReturnType;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskManagement.Domain.ValueObjects;


namespace TaskManagement.Application.Services.Base
{
    /// <summary>
    /// Provides base functionality for managing entities, including CRUD operations and pagination.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TReadDto">The DTO used to read the entity data.</typeparam>
    /// <typeparam name="TCreateDto">The DTO used for creating the entity.</typeparam>
    /// <typeparam name="TUpdateDto">The DTO used for updating the entity.</typeparam>
    public abstract class BaseBasicWithRelatedOneService<TKey, T, TRelated, TReadDto, TCreateDto, TUpdateDto> : BaseBasicEntityService<TKey, T, TReadDto, TCreateDto, TUpdateDto>, IBaseBasicWithRelatedOneService<TKey, T, TRelated, TReadDto, TCreateDto, TUpdateDto>
        where T : class, IBaseBasicEntity<TKey>
        where TReadDto : class, ILinks<TKey>
        where TRelated : class
    {
        // Injected dependencies
        protected readonly IActivityLog _activityLog;
        protected readonly IBaseBasicWithRelatedOneRepository<TKey, T, TRelated> _repository;
        protected readonly IMapper _mapper;
        protected readonly IEntityLinkGenerator _entityLinkGenerator;
        protected readonly IAppUserService _appUserService;
        protected readonly IValidator<TCreateDto> _createValidator;
        protected readonly IValidator<TUpdateDto> _updateValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBasicEntityService{TKey,T,TReadDto,TCreateDto,TUpdateDto}"/> class.
        /// </summary>
        /// <param name="activityLog">The activity log service.</param>
        /// <param name="repository">The repository to access entity data.</param>
        /// <param name="entityLinkGenerator">The generator for HATEOAS links.</param>
        /// <param name="appUserService">The service to manage application users.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        /// <param name="validator">The validator for update DTOs.</param>
        protected BaseBasicWithRelatedOneService(IActivityLog activityLog, IBaseBasicWithRelatedOneRepository<TKey, T, TRelated> repository, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IMapper mapper, IValidator<TCreateDto> createValidator, IValidator<TUpdateDto> updateValidator)
            : base(activityLog, repository, entityLinkGenerator, appUserService, mapper, createValidator, updateValidator)
        {
            _activityLog = activityLog;
            _repository = repository;
            _mapper = mapper;
            _entityLinkGenerator = entityLinkGenerator;
            _appUserService = appUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        #region CRUD Operations (Override base [Common])

        #endregion

        #region Common Methods

        #endregion

        #region CRUD Operations


        public async Task<TReadDto?> GetServiceAsync(TKey id)
        {
            //if (string.IsNullOrWhiteSpace(name))
            //{
            //    return new[] { BaseError<T>.MissingTitle };
            //}



            //Expression<Func<T, TRelated>> foreignKeyNavigation = entity =>
            //{
            //    var propertyInfo = GetPropertyInfo(entity, typeof(TRelated).Name);

            //    // Use 'as' because TRelated is now constrained to be a class type
            //    return propertyInfo as TRelated;
            //};

            string sdfsdaf = typeof(TRelated).Name;
            var foreignKeyExpression = BuildNavigationExpression<T, TRelated>(sdfsdaf);


            Expression<Func<T, TKey>>? primaryKeySelector = entity => entity.Id;

            T? ds= await _repository.GetWithRelatedOneAsync(id, foreignKeyExpression, primaryKeySelector); ;
            TReadDto entityReadDto = _mapper.Map<TReadDto>(ds);
            return entityReadDto;
        }

        #endregion

        public Expression<Func<T1, TRelated1>> BuildNavigationExpression<T1, TRelated1>(string propertyName)
        {
            // Create a parameter for the entity (e.g., entity => ...)
            var parameter = Expression.Parameter(typeof(T1), "entity");

            // Access the property dynamically (entity.PropertyName)
            var property = Expression.Property(parameter, propertyName);

            // Build the lambda (entity => entity.PropertyName)
            return Expression.Lambda<Func<T1, TRelated1>>(property, parameter);
        }

    }
}
