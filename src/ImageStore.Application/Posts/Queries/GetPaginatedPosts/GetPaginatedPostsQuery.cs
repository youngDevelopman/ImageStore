using ImageStore.Application.Models;

namespace ImageStore.Application.Posts.Queries.GetPaginatedPosts
{
    public record GetPaginatedPostsQuery(int PageSize, string Next, string Previous) : IRequest<CursorPaginatedResult<PostDto>>;
}
