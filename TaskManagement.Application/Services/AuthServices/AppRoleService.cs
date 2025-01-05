using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs.AuthDTOs.AppRole;
using TaskManagement.Domain.Entities.Auth;

namespace TaskManagement.Application.Services.AuthServices
{
    public class AppRoleService : IAppRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AppRoleService(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<List<AppRoleReadDto>> ListAsync()
        {
            var roleUsersMap = new List<AppRoleReadDto>();

            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                roleUsersMap.Add(new AppRoleReadDto { AppRole = role, AppUsers = usersInRole.ToList() });
            }

            return roleUsersMap;
        }

        public async Task<AppRoleReadDto> GetAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return null;

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            return new AppRoleReadDto { AppRole = role, AppUsers = usersInRole.ToList() };
        }

        public async Task<AppRole> CreateAsync(AppRoleCreateDto createRoleDto)
        {
            if (await _roleManager.RoleExistsAsync(createRoleDto.Name))
                return null;

            var role = new AppRole { Name = createRoleDto.Name };
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return null;

            return role;
        }

        public async Task<AppRole> UpdateAsync(string id, AppRoleUpdateDto updateRoleDto)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return null;

            role.Name = updateRoleDto.Name;
            var result = await _roleManager.UpdateAsync(role);

            //if (!result.Succeeded)
            //    return result.Errors;

            return role;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return false;

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return false;// result.Errors;

            return true;
        }
    }
}
