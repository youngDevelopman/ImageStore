namespace ImageStore.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken);
        Task AddCommentAsync(Comment comment, CancellationToken cancellationToken);
        void RemoveComment(Comment comment, CancellationToken cancellationToken);
    }
}
