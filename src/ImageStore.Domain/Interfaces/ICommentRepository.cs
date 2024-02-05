namespace ImageStore.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment Comment, CancellationToken cancellationToken);
    }
}
