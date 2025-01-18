// Third-party libraries
using AutoMapper;
using FluentValidation;

// Domain/Core layer
using TaskManagement.Domain.Entities.Base.Common;
using TaskManagement.Domain.Repositories.IBase;
using TaskManagement.Domain.Common.Pagination;
using TaskManagement.Domain.Common.AuditLog;
using TaskManagement.Domain.Common.HATEOAS;

// Application Layer
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Application.Utilities.Pagination;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Domain.Errors.Base;
using System.Linq.Expressions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Errors;
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
    public abstract class BaseCommonEntityService<TKey, T, TReadDto, TCreateDto, TUpdateDto> : IBaseCommonEntityService<TKey, T, TReadDto, TCreateDto, TUpdateDto>
        where T : class, IBaseCommonEntity<TKey>
        where TReadDto : class, ILinks<TKey>
    {
        // Injected dependencies
        protected readonly IActivityLog _activityLog;
        protected readonly IBaseCommonRepository<TKey, T> _repository;
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
        /// <param name="updateValidator">The validator for update DTOs.</param>
        protected BaseCommonEntityService(IActivityLog activityLog, IBaseCommonRepository<TKey, T> repository, IEntityLinkGenerator entityLinkGenerator, IAppUserService appUserService, IMapper mapper, IValidator<TCreateDto> createValidator, IValidator<TUpdateDto> updateValidator)
        {
            _activityLog = activityLog;
            _repository = repository;
            _mapper = mapper;
            _entityLinkGenerator = entityLinkGenerator;
            _appUserService = appUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

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
        public virtual async Task<OptionResult<IPaginatedList<TReadDto>>> ListAsync(ListFilter listFilter, Expression<Func<T, bool>>? filter = null)
        {
            IPaginatedList<T>? list = await _repository.ListAsync(listFilter, filter);

            if (list == null)
            {
                return new[] { BaseError<T>.NotFound };
            };

            // Map Entity to EntityReadDto
            var readDtos = list.Items.Select(entityPrototype =>
            {
                var readDto = _mapper.Map<TReadDto>(entityPrototype);

                _entityLinkGenerator.GenerateHateoasLinks<TKey, TReadDto>(readDto, readDto.Id.ToString());

                return readDto;
            }).ToList();

            _entityLinkGenerator.PaginationLinks<T>(list, listFilter.SearchTerm, listFilter.SortColumn, listFilter.SortOrder);

            // Return a new paginated list of EntityPrototypeReadDto with pagination data and HATEOAS links
            var paginatedResult = new PaginatedList<TReadDto>(readDtos, list.Page, list.PageSize, list.TotalCount)
            {
                Links = list.Links
            };

            return paginatedResult;
        }

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The <typeparamref name="TReadDto"/> representing the entity, or an error if not found.</returns>
        public virtual async Task<OptionResult<TReadDto>> GetAsync(TKey id)
        {
            var existingEntity = await _repository.GetAsync(id);

            if (existingEntity == null)
            {
                return new[] { BaseError<T>.NotFound };
            }

            TReadDto entityReadDto = _mapper.Map<TReadDto>(existingEntity);

            if (entityReadDto == null)
            {
                return new[] { BaseError<T>.ConversionFailed };
            }
            else
            {
                _entityLinkGenerator.GenerateHateoasLinks<TKey, TReadDto>(entityReadDto);
            }

            return entityReadDto;
        }

        /// <summary>
        /// Creates a new entity based on the provided DTO.
        /// </summary>
        /// <param name="createDto">The DTO containing the data to create the entity.</param>
        /// <returns>The ID of the newly created entity.</returns>
        public virtual async Task<OptionResult<TKey>> CreateAsync(TCreateDto createDto, Expression<Func<T, bool>>? predicate = null, string idFieldName = "Id")
        {
            #region Validate input

            var validationResult = await _createValidator.ValidateAsync(createDto);

            if (!validationResult.IsValid)
            {
                return new[] { new Error(400, BaseError<T>.InvalidData.ToString(), validationResult.Errors.ToString()!) };
            }

            #endregion

            #region Check if Name property (unique field) value exists or not

            if (predicate != null)
            {
                bool exists = await _repository.Exists(predicate);

                if (exists)
                {
                    return new[] { BaseError<T>.AlreadyExists };
                }
            }

            #endregion

            T entity = _mapper.Map<T>(createDto);

            #region Generate a new ID and set in the T entity

            if (typeof(TKey) == typeof(Guid))
            {
                // Assuming `Id` is a property of T and is of type TKey
                var propertyInfo = typeof(T).GetProperty(idFieldName);
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

            var entityId = typeof(T).GetProperty(idFieldName)?.GetValue(entity);

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
        public virtual async Task<OptionResult<TUpdateDto>> UpdateAsync(TKey id, TUpdateDto updateDto, Expression<Func<T, bool>>? predicate = null)
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

            if (predicate != null)
            {
                bool exists = await _repository.Exists(predicate);

                if (exists)
                {
                    return new[] { BaseError<T>.AlreadyExists };
                }
            }

            #endregion

            _mapper.Map(updateDto, existingEntity);

            int result = await _repository.UpdateAsync(existingEntity);

            if (result == 0)
            {
                return new[] { BaseError<T>.NoRowsAffected };
            }

            _mapper.Map(existingEntity, updateDto);

            return updateDto;
        }

        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>True if the entity was deleted successfully, otherwise false.</returns>
        public virtual async Task<OptionResult<bool>> DeleteAsync(TKey id)
        {
            var existingEntity = await _repository.GetAsync(id);
            if (existingEntity == null)
            {
                return new[] { BaseError<T>.NotFound };
            }

            int result = await _repository.DeleteAsync(existingEntity);

            if (result == 0)
            {
                return new[] { BaseError<T>.NoRowsAffected };
            }

            return result > 0;
        }

        #endregion

        #region Common Methods

        public async Task<OptionResult<bool>> Exists(TKey id)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                return new[] { BaseError<T>.MissingId };
            }

            var existingEntity = await _repository.GetAsync(id);

            if (existingEntity == null)
            {
                return new[] { BaseError<T>.NotFound };
            }

            return true;
        }

        #endregion
    }
}
