namespace ImageStore.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Caption { get; set; }
        public string Image { get; set; }
        // TODO: Consider using UserId
        public string Creator { get; set; }
        // TODO: Consider Lazy loading
        //public ICollection<Comment> Comments { get; set; }
    }
}
