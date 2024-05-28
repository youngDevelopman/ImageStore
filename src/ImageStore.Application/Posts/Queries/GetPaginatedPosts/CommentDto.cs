namespace ImageStore.Application.Posts.Queries.GetPaginatedPosts
{
    public class CommentDto
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
