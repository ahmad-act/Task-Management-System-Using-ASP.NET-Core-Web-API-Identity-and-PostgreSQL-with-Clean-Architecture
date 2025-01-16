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
        protected readonly IBaseBasicRepository<T> _repository;
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
        protected BaseBasicEntityService(IActivityLog activityLog, IBaseBasicRepository<T> repository, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IMapper mapper, IValidator<TCreateDto> createValidator, IValidator<TUpdateDto> updateValidator)
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

        public async Task<OptionResult<bool>> ExistsByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { BaseError<T>.MissingTitle };
            }

            Expression<Func<T, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.Exists(predicate);
        }

        public async Task<OptionResult<T?>> GetByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { BaseError<T>.MissingTitle };
            }

            Expression<Func<T, bool>> predicate = entity => entity.Name.Contains(name);
            return await _repository.GetByCondition(predicate);
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Retrieves a paginated list of entities with optional filtering, sorting, and pagination.
        /// </summary>
        /// <param name="listFilter">
        /// An instance of <see cref="ListFilter"/> containing optional parameters:
        /// <list type="bullet">
        /// <item>
        /// <description><c>SearchTerm</c>: Optional search term for filtering results.</description>
        /// </item>
        /// <item>
        /// <description><c>Page</c>: Optional page number for pagination. Must be greater than 0.</description>
        /// </item>
        /// <item>
        /// <description><c>PageSize</c>: Optional page size for pagination. Must be between 1 and 100.</description>
        /// </item>
        /// <item>
        /// <description><c>SortColumn</c>: Optional column to sort by. Maximum length is 50 characters.</description>
        /// </item>
        /// <item>
        /// <description><c>SortOrder</c>: Optional sort order. Valid values are "asc" or "desc".</description>
        /// </item>
        /// </list>
        /// </param>
        /// <typeparam name="TReadDto">The type of the data transfer object used to represent the entities.</typeparam>
        /// <typeparam name="TKey">The type of the unique identifier for entities.</typeparam>
        /// <returns>
        /// An <see cref="OptionResult{T}"/> containing:
        /// <list type="bullet">
        /// <item>
        /// <description>A paginated list of <typeparamref name="TReadDto"/> representing the entities if successful.</description>
        /// </item>
        /// <item>
        /// <description>An error code and message if the operation fails.</description>
        /// </item>
        /// </list>
        /// </returns>
        public async Task<OptionResult<IPaginatedList<TReadDto>>> ListAsync(ListFilter listFilter, Expression<Func<T, bool>>? filter = null)
        {
            Expression<Func<T, bool>>? defaultfilter = null;

            if (!string.IsNullOrWhiteSpace(listFilter.SearchTerm))
            {
                // Filter where Name AND Description contain "Search text"
                filter = x => x.Name.Contains(listFilter.SearchTerm) || (x.Description != null && x.Description.Contains(listFilter.SearchTerm));
            }

            return await base.ListAsync(listFilter, filter ?? defaultfilter);

            //IPaginatedList<T>? list = await _repository.ListAsync(listFilter, filter ?? defaultfilter);

            //if (list == null)
            //{
            //    return BaseError<T>.NotFound;
            //};

            //// Map Entity to EntityReadDto
            //var readDtos = list.Items.Select(entityPrototype =>
            //{
            //    var readDto = _mapper.Map<TReadDto>(entityPrototype);

            //    _entityLinkGenerator.GenerateHateoasLinks<TReadDto, TKey>(readDto, readDto.Id.ToString());

            //    return readDto;
            //}).ToList();

            //_entityLinkGenerator.PaginationLinks<T>(list, listFilter.SearchTerm, listFilter.SortColumn, listFilter.SortOrder);

            //// Return a new paginated list of EntityPrototypeReadDto with pagination data and HATEOAS links
            //var paginatedResult = new PaginatedList<TReadDto>(readDtos, list.Page, list.PageSize, list.TotalCount)
            //{
            //    Links = list.Links
            //};

            //return paginatedResult;
        }

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
                return new[] { new Error(400, BaseError<T>.InvalidData.ToString(), validationResult.Errors.ToString()!) };
            }

            #endregion

            #region Check if Name property (unique field) value exists or not

            string name = string.Empty;
            var propertyNameInfo = typeof(TCreateDto).GetProperty(nameof(IBaseBasicEntity<TKey>.Name));
            if (propertyNameInfo != null && propertyNameInfo.CanRead)
            {
                object? value = propertyNameInfo.GetValue(createDto);

                if (value != null && value is string)
                {
                    name = (string)value;
                }
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { BaseError<T>.MissingUniqueProperty };
            }

            Expression<Func<T, bool>> predicate = entity => entity.Name == name;

            bool exists = await _repository.Exists(predicate);

            if (exists)
            {
                return new[] { BaseError<T>.AlreadyExists };
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
                return new[] { BaseError<T>.NoRowsAffected };
            }

            #region Return the generated ID if successful, otherwise return Guid.Empty for Guid type or default for other types

            var entityId = typeof(T).GetProperty(nameof(IBaseBasicEntity<TKey>.Id))?.GetValue(entity);

            if (entityId == null)
            {
                return new[] { BaseError<T>.MissingId };
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
                return new[] { new Error(400, BaseError<T>.InvalidData.ToString(), validationResult.Errors.ToString()!) };
            }

            #endregion

            #region Check by ID if the entity exits

            var existingEntity = await _repository.GetAsync(id);

            if (existingEntity == null)
            {
                return new[] { BaseError<T>.NotFound };
            }

            #endregion

            #region Check if Name property (unique field) value exists or not

            string name = string.Empty;
            var propertyNameInfo = typeof(TUpdateDto).GetProperty(nameof(IBaseBasicEntity<TKey>.Name));
            if (propertyNameInfo != null && propertyNameInfo.CanRead)
            {
                object? value = propertyNameInfo.GetValue(updateDto);

                if (value != null && value is string)
                {
                    name = (string)value;
                }
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return new[] { BaseError<T>.MissingUniqueProperty };
            }

            if (existingEntity.Name != name)
            {
                Expression<Func<T, bool>> predicate = entity => entity.Name == name;

                bool exists = await _repository.Exists(predicate);

                if (exists)
                {
                    return new[] { BaseError<T>.AlreadyExists };
                }
            }

            #endregion

            updateDto.UpdateEntity(existingEntity);

            int result = await _repository.UpdateAsync(existingEntity);

            if (result == 0)
            {
                return new[] { BaseError<T>.NoRowsAffected };
            }

            _mapper.Map(existingEntity, updateDto);

            return updateDto;
        }

        #endregion
    }
}
