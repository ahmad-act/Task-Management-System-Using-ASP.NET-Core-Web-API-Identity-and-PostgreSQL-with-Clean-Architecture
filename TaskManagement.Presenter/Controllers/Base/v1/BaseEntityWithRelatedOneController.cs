// System
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// Third-party libraries
using Asp.Versioning;

// Domain/Core layer
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Common.HATEOAS;
using TaskManagement.Domain.Common.Pagination;
using TaskManagement.Domain.Common.ExtensionMethods;

// Application Layer
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Application.DTOs.BaseDTOs;

// Presenter Layer
using TaskManagement.Presenter.Filters;
using TaskManagement.Application.Utilities.Pagination;
using TaskManagement.Domain.Entities.Base.Common;
using TaskManagement.Domain.Entities.Base.Basic;
using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Common.ExtensionMethods;

namespace TaskManagement.Presenter.Controllers.Base.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [Produces("application/json")]
    //[Authorize]
    public class BaseEntityWithRelatedOneController<TKey, T, TRelated, TReadDto, TCreateDto, TUpdateDto> : ControllerBase
        where T : IBaseBasicEntity<Guid>
        where TReadDto : ILinks<Guid>
        where TCreateDto : IBaseCreateDto
        where TUpdateDto : IBaseUpdateDto
    {
        public readonly IBaseEntityWithRelatedOneService<TKey, T, TRelated, TReadDto, TCreateDto, TUpdateDto> _service;
        public readonly JwtSettings _jwtSettings;

        public BaseEntityWithRelatedOneController(IBaseEntityWithRelatedOneService<TKey, T, TRelated, TReadDto, TCreateDto, TUpdateDto> service, JwtSettings jwtSettings)
        {
            _service = service;
            _jwtSettings = jwtSettings;
        }

        #region CURD Operations

        [HttpGet]
        [InputValidation(ErrorMessage = "Please check your input and try again.", LogValidation = false)]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = $"Get a list of {nameof(T)}s", Description = $"Retrieves a list of {nameof(T)}s.")]
        [SwaggerResponse(200, $"List of {nameof(T)} retrieved successfully", typeof(IPaginatedList<object>))]
        [SwaggerResponse(404, $"{nameof(T)} not found")]
        [SwaggerResponse(401, "Invalid token")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> ListAsync([FromQuery] ListFilter listFilter)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Log or inspect the errors
                }

                return BadRequest(ModelState);
            }

            var result = await _service.ListWithRelatedOneAsync(listFilter);

            return result.Match<IActionResult>(
                value => Ok(ApiResponse.Success(value, "Data retrieved successfully")),  // Success: return HTTP 200
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        401 => Unauthorized(ApiResponse.Failure(error)),
                        404 => NotFound(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = $"Get a {nameof(T)} by ID", Description = $"Retrieves the details of a specific {nameof(T)} using their unique ID.")]
        [SwaggerResponse(200, $"{nameof(T)} retrieved successfully", typeof(object))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(401, "Invalid token")]
        [SwaggerResponse(404, $"{nameof(T)} not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetAsync(string id)
        {
            OptionResult<TReadDto> result = await _service.GetWithRelatedOneAsync(id.ToKey<TKey>());

            return result.Match<IActionResult>(
                value => Ok(ApiResponse.Success(value, "Data retrieved successfully")),          // Success: return HTTP 200
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        400 => BadRequest(ApiResponse.Failure(error)),
                        404 => NotFound(ApiResponse.Failure(error)),
                        401 => Unauthorized(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );
        }

        [HttpPost()]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = $"Add a new {nameof(T)}", Description = $"Creates a new {nameof(T)}.")]
        [SwaggerResponse(201, $"{nameof(T)} added successfully", typeof(object))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(401, "Invalid token")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> CreateAsync([FromBody] TCreateDto entityCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OptionResult<TKey> result = await _service.CreateAsync(entityCreate);

            return result.Match<IActionResult>(
                value => Created(string.Empty, ApiResponse.Success(value, "Data added successfully")),          // Success: return HTTP 200
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        400 => BadRequest(ApiResponse.Failure(error)),
                        401 => Unauthorized(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = $"Update a {nameof(T)}", Description = $"Updates the details of a specific a{nameof(T)}.")]
        [SwaggerResponse(200, $"{nameof(T)} updated successfully", typeof(object))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(401, "Invalid token")]
        [SwaggerResponse(404, $"{nameof(T)} not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] TUpdateDto entityUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"Invalid input: {typeof(T).Name} ID cannot be empty or whitespace.");
            }

            OptionResult<TUpdateDto> result = await _service.UpdateAsync(id.ToKey<TKey>(), entityUpdate);

            return result.Match<IActionResult>(
                value => Ok(ApiResponse.Success(value, "Data updated successfully")),          // Success: return HTTP 200
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        400 => BadRequest(ApiResponse.Failure(error)),
                        401 => Unauthorized(ApiResponse.Failure(error)),
                        404 => NotFound(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = $"Delete a {nameof(T)}", Description = $"Deletes a specific {nameof(T)} by their ID.")]
        [SwaggerResponse(200, $"{nameof(T)} deleted successfully", typeof(object))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(401, "Invalid token")]
        [SwaggerResponse(404, $"{nameof(T)} not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new KeyNotFoundException($"Invalid input: {typeof(T).Name} ID cannot be empty or whitespace.");
            }

            OptionResult<bool> result = await _service.DeleteAsync(id.ToKey<TKey>());

            return result.Match<IActionResult>(
                value => Ok(ApiResponse.Success(value, "Data deleted successfully")),          // Success: return HTTP 200
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        400 => BadRequest(ApiResponse.Failure(error)),
                        401 => Unauthorized(ApiResponse.Failure(error)),
                        404 => NotFound(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );
        }

        #endregion
    }
}