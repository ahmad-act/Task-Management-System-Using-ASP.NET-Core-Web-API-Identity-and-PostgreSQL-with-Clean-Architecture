﻿// Third-party libraries
using AutoMapper;
using FluentValidation;

// Domain/Core layer
using TaskManagement.Domain.Entities.Base.Basic;
using TaskManagement.Domain.Repositories.IBase;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Common.AuditLog;
using TaskManagement.Domain.Common.HATEOAS;

// Application Layer
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Domain.Errors.Base;
using System.Linq.Expressions;


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
    public abstract class BaseBasicEntityService<TKey, T, TReadDto, TCreateDto, TUpdateDto> : BaseCommonEntityService<TKey, T, TReadDto, TCreateDto, TUpdateDto>, IBaseBasicEntityService<TKey, T, TReadDto, TCreateDto, TUpdateDto>
        where T : class, IBaseBasicEntity<TKey>
        where TReadDto : class, ILinks<TKey>
    {
        // Injected dependencies
        protected readonly IActivityLog _activityLog;
        protected readonly IBaseCommonRepository<T> _repository;
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
        protected BaseBasicEntityService(IActivityLog activityLog, IBaseCommonRepository<T> repository, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IMapper mapper, IValidator<TCreateDto> createValidator, IValidator<TUpdateDto> updateValidator)
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

        #region CRUD Operations (Override base)

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Creates a new entity based on the provided DTO.
        /// </summary>
        /// <param name="createDto">The DTO containing the data to create the entity.</param>
        /// <returns>The ID of the newly created entity.</returns>
        public async Task<OptionResult<TKey>> CreateAsync(TCreateDto createDto)
        {
            #region Validate input

            var validationResult = await _createValidator.ValidateAsync(createDto);

            if (!validationResult.IsValid)
            {
                return new Error(400, BaseError<T>.InvalidData.ToString(), validationResult.Errors.ToString()!);
            }

            #endregion

            #region Check if Name property (unique field) value exists or not

            string name = string.Empty;
            var propertyNameInfo = typeof(TCreateDto).GetProperty(nameof(IBaseBasicEntity<TKey>.Name));
            if (propertyNameInfo != null && propertyNameInfo.CanRead)
            {
                object value = propertyNameInfo.GetValue(createDto);

                if (value is string)
                {
                    name = (string)value;
                }
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return BaseError<T>.MissingUniqueProperty;
            }

            Expression<Func<T, bool>> predicate = entity => entity.Name.Contains(name);

            bool exists = await _repository.Exists(predicate);

            if (exists)
            {
                return BaseError<T>.AlreadyExists;
            }

            #endregion

            T entity = _mapper.Map<T>(createDto);

            #region Generate a new ID and set in the T entity

            if (typeof(TKey) == typeof(Guid))
            {
                // Assuming `Id` is a property of T and is of type TKey
                var propertyInfo = typeof(T).GetProperty(nameof(IBaseBasicEntity<TKey>.Id));
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(entity, (TKey)(object)Guid.NewGuid());
                }
            }

            #endregion

            int result = await _repository.CreateAsync(entity);

            if (result == 0)
            {
                return BaseError<T>.NoRowsAffected;
            }

            #region Return the generated ID if successful, otherwise return Guid.Empty for Guid type or default for other types

            var entityId = typeof(T).GetProperty(nameof(IBaseBasicEntity<TKey>.Id))?.GetValue(entity);

            if (entityId == null)
            {
                return BaseError<T>.MissingId;
            }

            #endregion

            return (TKey)entityId;
        }

        /// <summary>
        /// Updates an existing entity based on the provided ID and update DTO.
        /// </summary>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="updateDto">The DTO containing the updated data for the entity.</param>
        /// <returns>The updated DTO.</returns>
        public virtual async Task<OptionResult<TUpdateDto>> UpdateAsync(string id, TUpdateDto updateDto)
        {
            #region Validate input

            var validationResult = await _updateValidator.ValidateAsync(updateDto);

            if (!validationResult.IsValid)
            {
                return new Error(400, BaseError<T>.InvalidData.ToString(), validationResult.Errors.ToString()!);
            }

            #endregion

            #region Check by ID if the entity exits

            var existingEntity = await _repository.GetAsync(id);

            if (existingEntity == null)
            {
                return BaseError<T>.NotFound;
            }

            #endregion

            #region Check if Name property (unique field) value exists or not

            string name = string.Empty;
            var propertyNameInfo = typeof(TUpdateDto).GetProperty(nameof(IBaseBasicEntity<TKey>.Name));
            if (propertyNameInfo != null && propertyNameInfo.CanRead)
            {
                object value = propertyNameInfo.GetValue(updateDto);

                if (value is string)
                {
                    name = (string)value;
                }
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return BaseError<T>.MissingUniqueProperty;
            }

            Expression<Func<T, bool>> predicate = entity => entity.Name.Contains(name);

            bool exists = await _repository.Exists(predicate);

            if (exists)
            {
                return BaseError<T>.AlreadyExists;
            }

            #endregion

            _mapper.Map(updateDto, existingEntity);

            int result = await _repository.UpdateAsync(existingEntity);

            if (result == 0)
            {
                return BaseError<T>.NoRowsAffected;
            }

            _mapper.Map(existingEntity, updateDto);

            return updateDto;
        }

        #endregion

    }
}
