using Amazon.S3;
using Amazon.S3.Model;
using ImageStore.Domain.Entities;
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

        public async Task UploadFileAsync(Stream fileStream, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<string, object>()
            {
                {  "Test", "This is a test value" },
            };
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = "image-store-test-app",
                Key = $"original/{Guid.NewGuid()}.jpeg",
                InputStream = fileStream,
            };
            putObjectRequest.Metadata.Add("Test", "This is a test value");

            await _s3Client.PutObjectAsync(putObjectRequest);
            //await _s3Client.UploadObjectFromStreamAsync("image-store-test-app", $"original/{Guid.NewGuid()}.jpeg", fileStream, dict, cancellationToken); ; ;
        }
    }
}
