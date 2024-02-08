using ImageStore.Domain.Entities;

namespace ImageStore.Application.Posts.Queries.GetPaginatedPosts
{
    public class PostDto
    {
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string Caption { get; set; }
        public string Image { get; set; }
        public int CommentsCount { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
    }
}
