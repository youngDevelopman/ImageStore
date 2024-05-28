namespace ImageStore.Domain.Interfaces
{
    public interface IPostRepository
    {
        Task AddPostRequestAsync(PostRequest postRequest, CancellationToken cancellationToken);
    }
}
