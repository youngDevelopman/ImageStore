using Amazon.S3;
using Amazon.S3.Model;
using ImageStore.Domain.Interfaces;
using ImageStore.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ImageStore.Infrastructure.ImageStore
{
    public class S3ImageStore : IImageStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Configuration _configuration;
        public S3ImageStore(IAmazonS3 s3Client, IOptions<S3Configuration> configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration.Value;
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = _configuration.Bucket,
                Key = $"{_configuration.OriginalImageFolder}/{fileName}", 
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
