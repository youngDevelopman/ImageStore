namespace ImageStore.Domain.Interfaces
{
    public interface IImageStorage
    {
        Task UploadFileAsync(Stream fileStream, CancellationToken cancellationToken);
    }
}
