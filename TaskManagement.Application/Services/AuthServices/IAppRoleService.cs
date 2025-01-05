using TaskManagement.Application.DTOs.AuthDTOs.AppRole;
using TaskManagement.Domain.Entities.Auth;

namespace TaskManagement.Application.Services.AuthServices
{
    public interface IAppRoleService
    {
        Task<AppRole> CreateAsync(AppRoleCreateDto createRoleDto);
        Task<bool> DeleteAsync(string id);
        Task<AppRoleReadDto> GetAsync(string id);
        Task<List<AppRoleReadDto>> ListAsync();
        Task<AppRole> UpdateAsync(string id, AppRoleUpdateDto updateRoleDto);
    }
}