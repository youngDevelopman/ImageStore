namespace ImageStore.Domain.Interfaces
{
    public interface IPostRepository
    {
        Task AddPostAsync(Post post, CancellationToken cancellationToken);
        Task AddPostRequestAsync(PostRequest postRequest, CancellationToken cancellationToken);
        Task<PostRequest> GetPostRequestByIdAsync(Guid postRequestId, CancellationToken cancellationToken);
    }
}
