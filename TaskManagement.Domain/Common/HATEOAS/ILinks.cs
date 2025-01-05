namespace TaskManagement.Domain.Common.HATEOAS
{
    public interface ILinks<TKey>
    {
        TKey? Id { get; set; }
        IList<Link> Links { get; set; }
    }
}