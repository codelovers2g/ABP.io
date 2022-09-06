using Volo.Abp.Domain.Entities;

namespace UrvinFinance.CommunityService.Comments;

[Serializable]
public class CommentWithDetailsDto : IHasConcurrencyStamp
{
    public Guid Id { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string Text { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool IsDeleted { get; set; }
    public List<CommentDto> Replies { get; set; }
    public UserDto Author { get; set; }
    public string ConcurrencyStamp { get; set; }
}
