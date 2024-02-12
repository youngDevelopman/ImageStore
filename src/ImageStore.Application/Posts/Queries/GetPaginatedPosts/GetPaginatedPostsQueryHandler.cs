using ImageStore.Application.Models;

namespace ImageStore.Application.Posts.Queries.GetPaginatedPosts
{
    public class GetPaginatedPostsQueryHandler : IRequestHandler<GetPaginatedPostsQuery, CursorPaginatedResult<PostDto>>
    {
        private readonly IPostRepository _postRepository;
        public GetPaginatedPostsQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<CursorPaginatedResult<PostDto>> Handle(GetPaginatedPostsQuery request, CancellationToken cancellationToken)
        {
            (PostCursor? cursor, PaginationStrategy paginationStrategy) = ParseCursorEncodedData(request);

            var posts = await _postRepository.GetCursorPaginatedPostsAsync(cursor, paginationStrategy, request.PageSize, cancellationToken);

            (PostCursor? start, PostCursor? end) = GeneratePostCursors(posts);

            var result = new CursorPaginatedResult<PostDto>()
            {
                Start = start.GetBase64Encoded(),
                End = end.GetBase64Encoded(),
                Items = posts.Select(x =>
                {
                    return new PostDto()
                    {
                        Caption = x.Caption,
                        CommentsCount = x.CommentsCount,
                        CreatedAt = x.CreatedAt,
                        Image = x.Image,
                        UserId = x.UserId,
                        UserName = x.User.UserName,
                        Comments = x.Comments.Select(c =>
                        {
                            return new CommentDto()
                            {
                                Content = c.Content,
                                CreatedAt = c.CreatedAt,
                                UserId = c.UserId,
                            };
                        })
                    };
                })
            };

            return result;
        }

        private (PostCursor? start, PostCursor? end) GeneratePostCursors(List<Post> posts)
        {
            PostCursor? start = default;
            PostCursor? end = default;

            var first = posts.FirstOrDefault();
            if (first != null)
            {
                start = PostCursor.CreateFromPost(first);
            }

            var last = posts.LastOrDefault();
            if (last != null)
            {
                end = PostCursor.CreateFromPost(last);
            }

            return (start, end);
        }


        // TODO: Consider moving this code to the PostPaginationQueryBuilder,
        // so it would be solely its responsibility to define which pagination strategy to pick
        private (PostCursor? cursor, PaginationStrategy paginationStrategy) ParseCursorEncodedData(GetPaginatedPostsQuery request)
        {
            if(!string.IsNullOrEmpty(request.Next) && !string.IsNullOrEmpty(request.Previous))
            {
                throw new InvalidOperationException($"Unable to parse cursor data");     
            }

            if (!string.IsNullOrEmpty(request.Next))
            {
                return (PostCursor.CreateFromBase64UrlEncoded(request.Next), PaginationStrategy.NextPage);
            }

            if (!string.IsNullOrEmpty(request.Previous))
            {
                return (PostCursor.CreateFromBase64UrlEncoded(request.Previous), PaginationStrategy.PreviousPage);
            }

            return (null, PaginationStrategy.FirstPage);
        }
    }
}
