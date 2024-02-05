using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;

namespace ImageStore.Infrastructure.Database.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(Comment Comment, CancellationToken cancellationToken)
        {
            await _context.Comments.AddAsync(Comment);
        }
    }
}
