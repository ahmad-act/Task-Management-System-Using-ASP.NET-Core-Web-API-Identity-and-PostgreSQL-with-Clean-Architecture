using TaskManagement.Application.DTOs.Issue;
using TaskManagement.Application.ServiceInterfaces.IBase;
using TaskManagement.Domain.Common.ReturnType;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.ServiceInterfaces
{
    public interface IIssueService : IBaseEntityWithRelatedOneService<Guid, Issue, Project, IssueReadDto, IssueCreateDto, IssueUpdateDto>
    {
        Task<OptionResult<bool>> Exists(string id);
        Task<OptionResult<bool>> ExistsByUniqueName(string name);
        Task<OptionResult<Issue?>> GetByUniqueName(string name);        
    }
}
