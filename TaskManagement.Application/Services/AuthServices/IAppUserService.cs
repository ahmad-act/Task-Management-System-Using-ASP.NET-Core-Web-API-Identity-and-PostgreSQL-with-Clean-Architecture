﻿using TaskManagement.Application.DTOs.AuthDTOs.AppUser;
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Entities.Auth;

namespace TaskManagement.Application.Services.AuthServices
{
    public interface IAppUserService : IBaseCommonEntityService<Guid, AppUser, AppUserReadDto, AppUserCreateDto, AppUserUpdateDto>
    {
        Task<OptionResult<bool>> Exists(Guid id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<AppUserReadDto?>> GetByUniqueName(string name);
    }
}