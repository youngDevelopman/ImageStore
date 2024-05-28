using ImageStore.Domain.Interfaces;

namespace ImageStore.Infrastructure.ImageStore
{
    public class S3ImageStore : IImageStorage
    {
        public Task UploadFileAsync(Stream fileStream, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
