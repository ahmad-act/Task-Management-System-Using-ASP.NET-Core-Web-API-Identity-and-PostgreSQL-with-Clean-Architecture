using AutoMapper;
using TaskManagement.Application.DTOs.AuthDTOs.AppUser;
using TaskManagement.Application.DTOs.EntityPrototype;
using TaskManagement.Application.DTOs.Issue;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.DTOs.Workspace;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Entities.Auth;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Infrastructure.DataContext
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region User

            CreateMap<AppUser, AppUserReadDto>();
            CreateMap<AppUserReadDto, AppUser>();

            CreateMap<AppUser, AppUserCreateDto>();
            CreateMap<AppUserCreateDto, AppUser>();

            CreateMap<AppUser, AppUserUpdateDto>();
            CreateMap<AppUserUpdateDto, AppUser>();

            CreateMap<AppUserCreateDto, AppUserRegisterDto>();
            CreateMap<AppUserRegisterDto, AppUserCreateDto>();

            #endregion

            #region EntityPrototype

            CreateMap<EntityPrototype, EntityPrototypeReadDto>();
            CreateMap<EntityPrototypeReadDto, EntityPrototype>();

            CreateMap<EntityPrototype, EntityPrototypeCreateDto>();
            CreateMap<EntityPrototypeCreateDto, EntityPrototype>();

            CreateMap<EntityPrototype, EntityPrototypeUpdateDto>();
            CreateMap<EntityPrototypeUpdateDto, EntityPrototype>();

            #endregion

            #region Workspace

            CreateMap<Workspace, WorkspaceReadDto>();
            CreateMap<WorkspaceReadDto, Workspace>();

            CreateMap<Workspace, WorkspaceCreateDto>();
            CreateMap<WorkspaceCreateDto, Workspace>();

            CreateMap<Workspace, WorkspaceUpdateDto>();
            CreateMap<WorkspaceUpdateDto, Workspace>();

            #endregion

            #region Project

            CreateMap<Project, ProjectReadDto>();
            CreateMap<ProjectReadDto, Project>();

            CreateMap<Project, ProjectCreateDto>();
            CreateMap<ProjectCreateDto, Project>();

            CreateMap<Project, ProjectUpdateDto>();
            CreateMap<ProjectUpdateDto, Project>();

            #endregion

            #region Task

            CreateMap<Domain.Entities.Task, TaskReadDto>();
            CreateMap<TaskReadDto, Domain.Entities.Task>();

            CreateMap<Domain.Entities.Task, TaskCreateDto>();
            CreateMap<TaskCreateDto, Domain.Entities.Task>();

            CreateMap<Domain.Entities.Task, TaskUpdateDto>();
            CreateMap<TaskUpdateDto, Domain.Entities.Task>();

            #endregion

            #region Issue

            CreateMap<Issue, IssueReadDto>();
            CreateMap<IssueReadDto, Issue>();

            CreateMap<Issue, IssueCreateDto>();
            CreateMap<IssueCreateDto, Issue>();

            CreateMap<Issue, IssueUpdateDto>();
            CreateMap<IssueUpdateDto, Issue>();

            #endregion
        }
    }
}

// https://youtu.be/RsnEZdc3MrE

