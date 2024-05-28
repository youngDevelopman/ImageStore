namespace ImageStore.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
