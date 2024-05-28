using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;
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

        public async Task<PostRequest> GetPostRequestByIdAsync(Guid postRequestId, CancellationToken cancellationToken)
        {
            return await _context.PostRequests.FirstOrDefaultAsync(x => x.Id == postRequestId, cancellationToken);
        }
    }
}
