namespace ImageStore.Domain.Interfaces
{
    public interface IImageStorage
    {
        Task UploadFileAsync(Stream fileStream, string fileName, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken);
    }
}
