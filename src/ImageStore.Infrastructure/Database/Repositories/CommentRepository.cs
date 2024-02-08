using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace ImageStore.Infrastructure.Database.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> GetCommentByIdAsync(Guid CommentId, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .Include(x => x.Post)
                .FirstOrDefaultAsync(x => x.Id == CommentId);
        }

        public async Task AddCommentAsync(Comment Comment, CancellationToken cancellationToken)
        {
            await _context.Comments.AddAsync(Comment);
        }

        public void RemoveComment(Comment comment, CancellationToken cancellationToken)
        {
            _context.Comments.Remove(comment);
        }
    }
}
