using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TaskManagement.Application.DTOs.AuthDTOs.AppUser;
using TaskManagement.Application.Services.AuthServices;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Presenter.Controllers.Base.v1;

namespace TaskManagement.Presenter.Controllers.v1
{
    public class AppUserController : BaseCommonEntityController<Guid, AppUser, AppUserReadDto, AppUserCreateDto, AppUserUpdateDto>
    {
        private readonly IAppUserService _appUserService;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUserController(IAppUserService appUserService, JwtSettings jwtSettings, SignInManager<AppUser> signInManager)
            : base(appUserService, jwtSettings)
        {
            _appUserService = appUserService;
            _signInManager = signInManager;
        }

        #region Domain-Specific endpoints

        [HttpPost("register")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "Register a new user", Description = "Creates a new user in the system.")]
        [SwaggerResponse(201, "User registered successfully", typeof(object))]
        [SwaggerResponse(400, "Bad request - Invalid user data")]
        [SwaggerResponse(401, "Unauthorized - Invalid token")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> Register([FromBody] AppUserRegisterDto appUserRegisterDto, [FromServices] IMapper mapper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUserCreateDto entity = mapper.Map<AppUserCreateDto>(appUserRegisterDto);

            OptionResult<Guid> result = await _service.CreateAsync(entity);

            return result.Match<IActionResult>(
                value => Created(string.Empty, new { id = Convert.ToString(value), message = $"{nameof(AppUser)} registered successfully" }),          // Success: return HTTP 200
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

        [HttpPost("login")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "User login", Description = "Authenticates a user and returns a JWT token.")]
        [SwaggerResponse(200, "User logged in successfully", typeof(object))]
        [SwaggerResponse(401, "Invalid login attempt")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> Login([FromBody] AppUserLoginDto appUserLoginDto, [FromServices] JwtSettings jwtSettings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = await _signInManager.PasswordSignInAsync(appUserLoginDto.UserName, appUserLoginDto.Password, appUserLoginDto.RememberMe, false);

            if (!login.Succeeded)
            {
                return Unauthorized("Invalid login attempt.");
            }

            var result = await _appUserService.GetByUniqueName(appUserLoginDto.UserName);

            return result.Match<IActionResult>(
                value =>  // Success: return HTTP 200
                {
                    var rolesList = value.AppRoles;
                    string roleNames = string.Join(",", rolesList.Select(role => role.Name));
                    string jwt = jwtSettings.GenerateJwt(value.Id.ToString(), roleNames);

                    Response.Cookies.Append(_jwtSettings.JwtTokenName, jwt, _jwtSettings.GetCookieOption());

                    return Ok(jwt);

                    //return Ok("Successfully logged in.");
                },
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        400 => BadRequest(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );

        }

        [HttpPost("logout")]
        [MapToApiVersion("1.0")]
        [SwaggerOperation(Summary = "Logout a user", Description = "Logs the user out of the system.")]
        [SwaggerResponse(200, "User logged out successfully")]
        [SwaggerResponse(401, "Invalid token")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("User logged out successfully.");
        }

        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Get user profile", Description = "Retrieves the user profile based on the authenticated user.")]
        [SwaggerResponse(200, "User profile retrieved successfully")]
        [SwaggerResponse(401, "Unauthorized access")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid or missing user information.");
            }

            var result = await _appUserService.GetAsync(userId);

            return result.Match<IActionResult>(
                value => Ok(value),// Success: return HTTP 200
                error =>
                {
                    return error[0].StatusCode switch
                    {
                        400 => BadRequest(ApiResponse.Failure(error)),
                        500 => StatusCode(500, ApiResponse.Failure(error)),
                        _ => StatusCode(error[0].StatusCode, ApiResponse.Failure(error))
                    };
                }
            );
        }

        #endregion
    }
}
