using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using ImageStore.API.Configuration;
using ImageStore.API.Exceptions;
using ImageStore.Application.Exceptions;
using ImageStore.Application.Posts.Commands.CreatePost;
using ImageStore.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ImageStore.API.Services
{
    public class SQSProcessedImageProcessor : BackgroundService
    {
        private readonly IAmazonSQS _queue;
        private readonly IServiceProvider _services;
        private readonly ILogger<SQSProcessedImageProcessor> _logger;
        private readonly ProcessedImagesQueueConfig _configuration;
        public SQSProcessedImageProcessor(
            AwsCredentialsConfuguration awsCredentialsConfuguration, 
            IServiceProvider services,
            IOptions<ProcessedImagesQueueConfig> configuration,
            ILogger<SQSProcessedImageProcessor> logger)
        {
            _queue = new AmazonSQSClient(awsCredentialsConfuguration.Credentials, awsCredentialsConfuguration.Region);
            _services = services;
            _logger = logger;
            _configuration = configuration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"getting messages from the queue {DateTime.Now}");
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = _configuration.SqsUrl,
                    MaxNumberOfMessages = _configuration.MaxNumberOfMessages,
                    WaitTimeSeconds = _configuration.WaitTimeSeconds, // This parameter enables long polling for SQS
                };

                var response = await _queue.ReceiveMessageAsync(request);
                if (response.Messages.Any()) // Posibility of getting 0 messages
                {
                    string messageReceipt = default(string);
                    try
                    {
                        using var scope = _services.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
                        foreach (var message in response.Messages)
                        {
                            messageReceipt = message.ReceiptHandle;
                            var s3Event = S3EventNotification.ParseJson(message.Body);
                            var records = s3Event.Records;
                            foreach (var record in records)
                            {
                                var filePath = record.S3.Object.Key;
                                var bucket = record.S3.Bucket.Name;
                                var region = record.AwsRegion;

                                var imageUrl = new Uri($"https://{bucket}.s3.{region}.amazonaws.com/{filePath}");

                                var fileMetadata = await s3Client.GetObjectMetadataAsync(bucket, filePath);

                                var requestIdStr = fileMetadata.Metadata["post-request-id"];
                                if(requestIdStr == null )
                                {
                                    throw new RequestIdNotFound($"Request Id not found for {imageUrl}");
                                }
                                var requestId = Guid.Parse(requestIdStr);

                                await mediator.Send(new CreatePostCommand(requestId, imageUrl));
                                
                            }

                            _logger.LogInformation($"Message has been processed {message.Body}");
                            await _queue.DeleteMessageAsync(_configuration.SqsUrl, messageReceipt);
                        }
                    }
                    catch (RequestIdNotFound ex)
                    {
                        /* In this scenario, the request id record of the s3 file which is stored in the metadata has not been found,
                         so we want to put the message into DLQ */

                        // TODO: Put message to DLQ
                        _logger.LogError(ex.Message);
                        await _queue.DeleteMessageAsync(_configuration.SqsUrl, messageReceipt);
                    }
                    catch(Exception ex) when (ex is PostAlreadyExistsException or PostRequestNotFoundException)
                    {
                        /*
                         In this scenario we likely got duplicate messages in the queue, for example due to network partitioning, 
                            so we simply want to remove the duplicate messages from the queue 
                         */
                        _logger.LogError(ex.Message);
                        await _queue.DeleteMessageAsync(_configuration.SqsUrl, messageReceipt);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($" Unknown error {ex.Message}");
                    }
                }
            }
        }
    }
}
