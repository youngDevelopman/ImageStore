namespace ImageStore.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Caption { get; set; }
        public string Image { get; set; }
        // TODO: Consider using UserId
        public string Creator { get; set; }
        public int CommentsCount { get; set; }
        public byte[] Version { get; set; }

        // TODO: Consider Lazy loading and make it readonly
        public ICollection<Comment> Comments { get; set; }
    }
}
