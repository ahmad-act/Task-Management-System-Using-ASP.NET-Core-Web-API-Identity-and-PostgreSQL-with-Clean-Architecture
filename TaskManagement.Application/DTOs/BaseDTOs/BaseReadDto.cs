using TaskManagement.Domain.Common.HATEOAS;

namespace TaskManagement.Application.DTOs.BaseDTOs
{
    public abstract class BaseReadDto<TKey> : IBaseReadDto, ILinks<TKey>
    {
        public int? UserDataAccessLevel { get; set; }

        public TKey? Id { get; set; }
        public IList<Link> Links { get; set; } = new List<Link>();
    }
}
