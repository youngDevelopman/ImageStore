using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;

namespace ImageStore.Infrastructure.Database.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context)
        {
            _context = context;   
        }
        public async Task AddPostRequestAsync(PostRequest postRequest, CancellationToken cancellationToken)
        {
           await _context.PostRequests.AddAsync(postRequest, cancellationToken);
        }
    }
}
