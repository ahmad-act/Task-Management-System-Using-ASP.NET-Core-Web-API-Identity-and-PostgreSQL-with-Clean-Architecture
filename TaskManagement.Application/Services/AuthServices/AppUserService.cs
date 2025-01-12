using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using TaskManagement.Application.DTOs.AuthDTOs.AppUser;
using TaskManagement.Application.Utilities.Pagination;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Common.Pagination;
using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Domain.Errors;

namespace TaskManagement.Application.Services.AuthServices
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        protected readonly IMapper _mapper;

        public AppUserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        #region CRUD Operations

        public async Task<OptionResult<IPaginatedList<AppUserReadDto>>> ListAsync(ListFilter listFilter, Expression<Func<AppUser, bool>>? filter = null)
        {
            var userRolesMap = new List<AppUserReadDto>();

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var rolesInUser = await _userManager.GetRolesAsync(user);

                var roles = new List<AppRole>();
                foreach (var roleName in rolesInUser)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);

                    if (role != null)
                    {
                        roles.Add(role);
                    }
                }

                // Map user and roles to AppUserReadDto
                userRolesMap.Add(new AppUserReadDto { Id = user.Id, AppUser = user, AppRoles = roles });
            }

            // Apply filtering (searchTerm)
            if (!string.IsNullOrWhiteSpace(listFilter.SearchTerm))
            {
                userRolesMap = userRolesMap
                    .Where(urm => urm.AppUser.UserName.Contains(listFilter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  urm.AppRoles.Any(role => role.Name.Contains(listFilter.SearchTerm, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Pagination logic
            listFilter.Page ??= 1;
            listFilter.PageSize ??= int.MaxValue;

            var paginatedUsers = userRolesMap
                .Skip(Convert.ToInt32((listFilter.Page.Value - 1) * listFilter.PageSize.Value))
                .Take(Convert.ToInt32(listFilter.PageSize.Value))
                .ToList();

            // Sorting logic
            if (!string.IsNullOrWhiteSpace(listFilter.SortColumn))
            {
                var isDescending = string.Equals(listFilter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
                paginatedUsers = listFilter.SortColumn.ToLower() switch
                {
                    "username" => isDescending
                        ? paginatedUsers.OrderByDescending(urm => urm.AppUser.UserName).ToList()
                        : paginatedUsers.OrderBy(urm => urm.AppUser.UserName).ToList(),
                    "rolename" => isDescending
                        ? paginatedUsers.OrderByDescending(urm => urm.AppRoles.FirstOrDefault()?.Name).ToList()
                        : paginatedUsers.OrderBy(urm => urm.AppRoles.FirstOrDefault()?.Name).ToList(),
                    _ => paginatedUsers
                };
            }

            // Create paginated result
            var paginatedResult = new PaginatedList<AppUserReadDto>(
                paginatedUsers,
                listFilter.Page.Value,
                listFilter.PageSize.Value,
                (uint)userRolesMap.Count
            );

            return paginatedResult;
        }


        public async Task<OptionResult<AppUserReadDto>> GetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return AppUserError.NotFound;
            }

            var rolesInUser = await _userManager.GetRolesAsync(user);

            List<AppRole> roles = new List<AppRole>();
            foreach (var roleName in rolesInUser)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                    continue;

                roles.Add(role);
            }

            return new AppUserReadDto { AppUser = user, AppRoles = roles };
        }

        public async Task<OptionResult<Guid>> CreateAsync(AppUserCreateDto create, Expression<Func<AppUser, bool>>? predicate = null, string idFieldName = "Id")
        {
            if (create == null)
            {
                return AppUserError.Null;
            }

            if (string.IsNullOrWhiteSpace(create.UserName))
            {
                return AppUserError.MissingUserName;
            }

            var existingUser = await _userManager.FindByNameAsync(create.UserName);
            if (existingUser != null)
            {
                return AppUserError.AlreadyExists;
            }

            var user = new AppUser
            {
                UserName = create.UserName,
                Email = create.Email,
                FirstName = create.FirstName,
                LastName = create.LastName
            };

            var result = await _userManager.CreateAsync(user, create.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));

                return new Error(500, AppUserError.NoRowsAffected.Status, errors);
            }

            return user.Id;
        }

        public async Task<OptionResult<AppUserUpdateDto>> UpdateAsync(string id, AppUserUpdateDto update, Expression<Func<AppUser, bool>>? predicate = null)
        {
            var existingEntity = await _userManager.FindByIdAsync(id);
            if (existingEntity == null)
            {
                return AppUserError.NotFound;
                //return "404"; // Not Found
            }

            _mapper.Map(update, existingEntity);

            var result = await _userManager.UpdateAsync(existingEntity);
            if (!result.Succeeded)
            {
                return AppUserError.NoRowsAffected;
            }

            _mapper.Map(existingEntity, update);

            return update;
        }

        public async Task<OptionResult<bool>> DeleteAsync(string id)
        {
            var existingEntity = await _userManager.FindByIdAsync(id);

            if (existingEntity == null)
            {
                return AppUserError.NotFound;
            }

            var result = await _userManager.DeleteAsync(existingEntity);

            if (!result.Succeeded)
            {
                return AppUserError.NoRowsAffected;
            }

            return result.Succeeded;
        }

        #endregion

        #region Common Methods

        public async Task<OptionResult<bool>> Exists(string id)
        {
            var existingEntity = await _userManager.FindByIdAsync(id);

            if (existingEntity == null)
            {
                return AppUserError.NotFound;
            }

            return true;
        }

        public async Task<OptionResult<bool>> ExistsByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return AppUserError.MissingUserName;
            }

            var existingUser = await _userManager.FindByNameAsync(name);

            return existingUser != null;
        }

        public async Task<OptionResult<AppUserReadDto?>> GetByUniqueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return AppUserError.MissingTitle;
            }

            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                return AppUserError.NotFound;
            }

            var rolesInUser = await _userManager.GetRolesAsync(user);

            List<AppRole> roles = new List<AppRole>();
            foreach (var roleName in rolesInUser)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            var result = new AppUserReadDto
            {
                Id = user.Id,
                AppUser = user,
                AppRoles = roles
            };

            return result;
        }

        #endregion

        #region Domain-Specific Methods


        #endregion

    }
}
