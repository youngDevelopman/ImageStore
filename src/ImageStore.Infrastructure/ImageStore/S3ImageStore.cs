using Amazon.S3;
using Amazon.S3.Model;
using ImageStore.Domain.Interfaces;

namespace ImageStore.Infrastructure.ImageStore
{
    public class S3ImageStore : IImageStorage
    {
        private readonly IAmazonS3 _s3Client;
        public S3ImageStore(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = "image-store-test-app", // TODO: retrieve Bukcet name from config
                Key = $"original/{fileName}", //  TODO: retrieve folder name from config
                InputStream = fileStream,
            };

            foreach (var item in metadata)
            {
                putObjectRequest.Metadata.Add(item.Key, item.Value);
            }

            await _s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        }
    }
}
