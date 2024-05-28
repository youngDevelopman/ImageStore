namespace ImageStore.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid PostId { get; set; } 
        public string User { get; set; }
        public string Content { get; set; }
    }
}
