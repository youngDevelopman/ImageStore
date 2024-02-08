using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;
using ImageStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageStore.Infrastructure.Database.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context)
        {
            _context = context;   
        }

        public async Task AddPostAsync(Post post, CancellationToken cancellationToken)
        {
            await _context.Posts.AddAsync(post, cancellationToken);
        }

        public async Task AddPostRequestAsync(PostRequest postRequest, CancellationToken cancellationToken)
        {
           await _context.PostRequests.AddAsync(postRequest, cancellationToken);
        }

        public async Task<Post> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken)
        {
            return await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
        }

        public async Task<List<Post>> GetCursorPaginatedPostsAsync(PostCursor cursor, PaginationStrategy paginationStrategy, int pageSize, CancellationToken cancellationToken)
        {
            var queryBuilder = new PostPaginationQueryBuilder(_context);
            var query = queryBuilder
                .SetPaginationStrategy(paginationStrategy)
                .SetCursor(cursor)
                .WithSize(pageSize)
                .IncludeComments()
                .Build();

            var result = await query.ToListAsync();

            if(paginationStrategy == PaginationStrategy.PreviousPage)
            {
                result = result.OrderByDescending(x => x.CommentsCount)
                    .ThenByDescending(x => x.UpdatedAt)
                    .ThenByDescending(x => x.Id)
                    .ToList();
            }

            return result;
        }

        public async Task<PostRequest> GetPostRequestByIdAsync(Guid postRequestId, CancellationToken cancellationToken)
        {
            return await _context.PostRequests.FirstOrDefaultAsync(x => x.Id == postRequestId, cancellationToken);
        }
    }
}
