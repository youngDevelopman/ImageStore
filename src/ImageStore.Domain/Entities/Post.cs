namespace ImageStore.Domain.Entities
{
    public class Post : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Caption { get; set; }
        public string Image { get; set; }
        public int CommentsCount { get; set; }
        public byte[] Version { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
